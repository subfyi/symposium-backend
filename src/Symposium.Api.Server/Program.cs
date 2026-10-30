global using Symposium.Api.Server.Extensions;
global using Symposium.Storage;
global using Symposium.Models;
global using Symposium.Query.Projections;
global using Symposium.Api.Server.Utils;
global using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.Json.Serialization;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using Npgsql;
using Symposium.Api.Server.Converters;
using Symposium.Api.Server.Services;
using Symposium.Storage.Extensions;
using JustAuthenticator.Extensions;
using JustScheduler;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.Extensions.FileProviders;
using Symposium.Services.Extensions;
using Symposium.Services;
using Tamturk.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
StartupConfiguration.Validate(builder.Configuration);
builder.Host.UseSystemd();
builder.WebHost.UseKestrel(a => a.UseSystemd());

builder.Services.AddJustScheduler(_ => { });
builder.Services.AddHostedService<NotificationDispatcher>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("authentication", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = Math.Max(1, builder.Configuration.GetValue<int>("authentication:rateLimit", 20)),
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource
            .AddService(serviceName: "Symposium")
            .AddAttributes([
                new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName),
                new KeyValuePair<string, object>("service.instance.id", Environment.MachineName)
            ])
    )
    .WithLogging(logging => logging.AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddNpgsql()
        .AddOtlpExporter());

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
});

builder.Services
    .AddAuthorization()
    .AddJustAuthenticator(_builder => _builder
        .UseSymmetricKey(builder.Configuration["jwt"])
        .UseHandler<AuthenticatorService, Client, user>()
        .SetIssuer("Symposium")
        .SetAudience("Symposium Users")
        .SetExpiration(TimeSpan.FromHours(1))
        .UseAuthorizationOptions(a =>
        {
            a.AddPolicy("Admins",
                policy => policy.AddAuthenticationSchemes("Bearer").RequireRole("Admin")
                    .RequireAuthenticatedUser());
        })
        .Build());

if (builder.Configuration.GetSection("smtp") is { } smtp && smtp.Exists())
{
    if (!int.TryParse(smtp["port"], out var _smtp_port))
    {
        throw new Exception("SMTP server port is invalid");
    }

    var smtp_credentials =
        new NetworkCredential(builder.Configuration["smtp:user"], builder.Configuration["smtp:pass"]);

    builder.Services
        .AddSingleton(new MailAddress(smtp["from"]))
        .AddScoped(a => new SmtpClient(smtp["server"], _smtp_port)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = smtp_credentials,
            EnableSsl = smtp.GetValue<bool>("enableSsl", true)
        });
}
else if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddSingleton(new MailAddress("test@localhost"))
        .AddScoped(a => new SmtpClient("localhost", 1025)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = false,
        });
}
else
{
    throw new InvalidOperationException("SMTP configuration is missing");
}

builder.Services
    .AddServices()
    .AddSingleton(builder.Configuration)
    .AddRequestSigning(builder.Configuration["hash"])
    .AddStorage(builder.Configuration["database"], builder.Configuration.GetSection("storage"))
    .AddCors()
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new FileConverter(builder.Configuration["storage:cdn:url"]));
        options.JsonSerializerOptions.Converters.Insert(0, new DateTimeOffsetConverter(TimeZoneInfo.Utc));
    });

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Languages", policy => policy
        .Tag("languages")
        .Expire(TimeSpan.FromDays(365))
    );
});

builder.Services
    .AddHealthChecks()
    .AddNpgSql();

var app = builder.Build();
var allowedOrigins = builder.Configuration.GetSection("cors:origins").Get<string[]>() ??
    (builder.Environment.IsDevelopment()
        ? ["http://localhost:3000", "http://localhost:4000", "http://127.0.0.1:3000", "http://127.0.0.1:4000"]
        : ["https://iseser.com", "https://www.iseser.com", "https://portal.iseser.com"]);

using (var scope = app.Services.CreateScope())
{
    if (builder.Configuration.GetValue<bool>("databaseOptions:migrateOnStartup"))
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    if (builder.Configuration.GetValue<bool>("bootstrap-admin"))
    {
        await StartupConfiguration.CreateAdmin(scope.ServiceProvider, builder.Configuration);
        return;
    }
}

app.UsePathBase("/api");
if (builder.Configuration["legacyFiles:documentsRoot"] is { Length: > 0 } documentsRoot)
{
    if (!Path.IsPathFullyQualified(documentsRoot) || !Directory.Exists(documentsRoot))
        throw new InvalidOperationException("legacyFiles:documentsRoot must be an existing absolute public documents directory.");
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(documentsRoot),
        RequestPath = "/doc",
        OnPrepareResponse = context => context.Context.Response.Headers["X-Content-Type-Options"] = "nosniff"
    });
}

app
    .UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    })
    .UseCors(policy => policy.SetPreflightMaxAge(TimeSpan.FromMinutes(10)).AllowAnyMethod().WithOrigins(allowedOrigins)
        .AllowAnyHeader())
    .UseAuthentication()
    .Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception e) when (e is not BadHttpRequestException)
        {
            context.RequestServices.GetRequiredService<ILogger<Program>>()
                .LogError(e, "Error on request.");

            context.Response.StatusCode = e is SubmissionPermissionException ? 403 : e is SubmissionLockedException ? 409 :
                e is KeyNotFoundException || e is NotImplementedException ? 404 :
                e is UnauthorizedAccessException || e is SecurityTokenValidationException ? 401 :
                e is ArgumentException ? 400 : 500;
            context.Response.ContentType = "application/json; charset=utf-8";

            await context.Response.WriteAsJsonAsync(new
            {
                code = -1,
                message = context.Response.StatusCode == 500 && !app.Environment.IsDevelopment()
                    ? "An unexpected error occurred." : e.Message,
                stacktrace = app.Environment.IsDevelopment() ? e.StackTrace : null
            });
        }
    })
    .UseRouting()
    .UseRateLimiter()
    .UseOutputCache()
    .UseAuthorization()
    .UseEndpoints(a =>
    {
        a.MapHealthChecks("/healthz");
        a.MapJustAuthenticator("/token").RequireRateLimiting("authentication");

        a.MapControllers();
    });

app.Run();
