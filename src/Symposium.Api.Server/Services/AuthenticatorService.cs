using JustAuthenticator.Abstractions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Symposium.Api.Server.Services;

public class Client
{
    public Guid id { get; set; }

    public string password { get; set; } = "";

    public string client_id { get; set; } = "";
}

public class AuthenticatorService : IAuthenticatorService<Client, user>
{
    private readonly AppDbContext db;

    private readonly List<Client> clients;

    public AuthenticatorService(AppDbContext db, IConfiguration configuration)
    {
        this.db = db;
        clients = configuration.GetSection("authentication:clients").Get<List<Client>>() ?? [];
    }

    public Task<Client?> GetClient(string client_id, IPassword client_secret)
    {
        var client = clients.FirstOrDefault(a => a.client_id == client_id || a.id.ToString() == client_id);
        if (client == null)
            return Task.FromResult<Client?>(null);

        if (!client_secret.Compare(client.password))
            return Task.FromResult<Client?>(null);

        return Task.FromResult<Client?>(client);
    }

    public async Task<user?> GetUser(Client client, string username, IPassword password, bool trusted)
    {
        username = username.Trim().ToLowerInvariant();

        var user = await db.users.FirstOrDefaultAsync(a => a.email == username);
        if (user == null)
            return null;

        if (!password.Compare(user.password))
            return null;

        return user;
    }

    public Task<ClaimsIdentity> MakeClaims(Client client, user user)
    {
        var claims = new ClaimsIdentity();

        claims.AddClaim(new Claim("user_id", user.id.ToString()));
        claims.AddClaim(new Claim("type", user.type.ToString()));

        claims.AddClaim(new Claim(ClaimTypes.Role, user.type switch
        {
            UserType.ADMIN => "Admin",
            UserType.MEMBER => "Member",
            _ => throw new ArgumentOutOfRangeException()
        }));

        return Task.FromResult(claims);
    }

    public async Task SaveToken(Client client, user user, ICode token, bool disposable)
    {
        db.tokens.Add(new token
        {
            client_id = client.id,
            id = token.id,
            created_at = DateTimeOffset.UtcNow,
            shouldDispose = disposable,
            password = token.password.Hashed,
            user_id = user.id
        });

        await db.SaveChangesAsync();
    }

    public async Task<user?> ValidateToken(Client client, ICode token, bool dispose)
    {
        var code = await db.tokens
            .Include(a => a.user)
            .FirstOrDefaultAsync(a => a.client_id == client.id && a.id == token.id && a.shouldDispose == dispose);
        if (code == null)
            return null;

        if (!token.password.Compare(code.password))
            return null;

        if (dispose)
        {
            db.tokens.Remove(code);
            await db.SaveChangesAsync();
        }

        return code.user;
    }
}
