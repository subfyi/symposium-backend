using System.Net.Mail;
using JustAuthenticator.Abstractions;

namespace Symposium.Api.Server.Services;

public static class StartupConfiguration
{
    public static void Validate(IConfiguration configuration)
    {
        foreach (var key in new[] { "jwt", "hash" })
            if ((configuration[key]?.Length ?? 0) < 32 || configuration[key]!.StartsWith("REPLACE_", StringComparison.Ordinal))
                throw new InvalidOperationException($"Configuration '{key}' must contain at least 32 characters.");
        if (string.IsNullOrWhiteSpace(configuration["database"]))
            throw new InvalidOperationException("PostgreSQL 'database' configuration is required.");

        var clients = configuration.GetSection("authentication:clients").Get<List<Client>>() ?? [];
        if (clients.Count == 0 || clients.Any(a => a.id == Guid.Empty || string.IsNullOrWhiteSpace(a.client_id) ||
            string.IsNullOrWhiteSpace(a.password) || a.password.Length != 60 || !a.password.StartsWith("$2", StringComparison.Ordinal)) || clients.Select(a => a.id).Distinct().Count() != clients.Count ||
            clients.Select(a => a.client_id).Distinct(StringComparer.Ordinal).Count() != clients.Count)
            throw new InvalidOperationException("Configure unique authentication clients with GUID id, client_id and hashed password.");
    }

    public static async Task CreateAdmin(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["bootstrap:email"]?.Trim().ToLowerInvariant();
        var password = configuration["bootstrap:password"];
        if (email is null || !MailAddress.TryCreate(email, out var address) || address.Address != email ||
            (password?.Length ?? 0) < 12)
            throw new InvalidOperationException("Set bootstrap__email and bootstrap__password (at least 12 characters).");
        var db = services.GetRequiredService<AppDbContext>();
        if (await db.users.AnyAsync(a => a.email == email))
            throw new InvalidOperationException("This account already exists; no permissions were changed.");
        db.users.Add(new user
        {
            email = email,
            password = services.GetRequiredService<IPasswordProvider>().Generate(password!).Hashed,
            type = UserType.ADMIN,
            first_name = "Administrator",
            created_at = DateTimeOffset.UtcNow,
            password_set_at = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
