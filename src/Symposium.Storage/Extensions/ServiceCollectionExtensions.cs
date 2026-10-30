using EntityFrameworkCore.Projectables.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Symposium.Storage.Services;

namespace Symposium.Storage.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection that, string connectionString, IConfigurationSection storageConfig)
    {
        var server = storageConfig["server"];
        var username = storageConfig["username"];
        var password = storageConfig["password"];
        var cdnUrl = storageConfig["cdn:url"];
        var cdnToken = storageConfig["cdn:token"];

        return that.AddStorage(connectionString, server, username, password, cdnUrl, cdnToken);
    }

    public static IServiceCollection AddStorage(this IServiceCollection that, string connectionString, string server,
        string username, string password, string cdnUrl, string cdnToken)
    {
        var psql = new NpgsqlDataSourceBuilder(connectionString)
            .EnableDynamicJson()
            .Build();

        return that
            .AddSingleton(psql)
            .AddSingleton<WebDavClient>(a => new WebDavClient(server, username, password))
            .AddScoped<FileService>(a => new FileService(
                a.GetRequiredService<AppDbContext>(),
                a.GetRequiredService<WebDavClient>(),
                cdnUrl,
                cdnToken,
                a.GetRequiredService<ILogger<FileService>>()
            ))
            .AddDbContext<AppDbContext>(
                options => options
                    .ConfigureWarnings(a => a.Ignore(RelationalEventId.OptionalDependentWithAllNullPropertiesWarning))
                    .UseNpgsql(psql,
                        psql => psql.SetPostgresVersion(18, 1)
                            .MigrationsAssembly("Symposium.Storage")
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    )
                    .UseProjectables(a => a.CompatibilityMode(CompatibilityMode.Full)));
    }
}