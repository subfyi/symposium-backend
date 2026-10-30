using JustAuthenticator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Symposium.Api.Server.Controllers;

[Authorize]
[Route("user")]
[Route("kullanicilar")]
[Route("profile")]
public class UserController : ControllerBase
{
    private readonly AppDbContext db;
    private readonly IPasswordProvider passwordProvider;

    public UserController(AppDbContext db, IPasswordProvider passwordProvider)
    {
        this.db = db;
        this.passwordProvider = passwordProvider;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<Pagination<User>> all_users(string? query = null)
    {
        return await this.Paginate(db.users
            .Where(a => query == null || EF.Functions.ILike(a.first_name + " " + a.last_name, $"%{query}%") ||
                        EF.Functions.ILike(a.email, $"%{query}%"))
            , a => a.Select(a => a.ToUser()));
    }

    [HttpGet("me")]
    [HttpGet("{id:int}")]
    public async Task<User> single_user(int? id)
    {
        id ??= int.Parse(User.FindFirst("user_id").Value);
        if (!User.IsInRole("Admin") && id != int.Parse(User.FindFirst("user_id").Value))
            throw new UnauthorizedAccessException();

        var model = await db.users
            .Where(a => a.id == id)
            .Select(a => a.ToUser())
            .SingleAsync();
        var values = await db.Set<parameters>().Where(a => a.id == model.u_title || a.id == model.u_type || a.id == model.gender)
            .Select(a => new PublicArticleParameter { id = a.id, value = a.value }).ToListAsync();
        return model with
        {
            utitle = values.FirstOrDefault(a => a.id == model.u_title),
            utype = values.FirstOrDefault(a => a.id == model.u_type),
            ugender = values.FirstOrDefault(a => a.id == model.gender)
        };
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task delete_user(int? id)
    {
        await db.users
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPatch("me")]
    public async Task<User> update_user([FromBody] User model)
    {
        int id = int.Parse(this.User.FindFirst("user_id").Value);
        var entity = await db.users.SingleAsync(a => a.id == id);
        await ValidateEmail(model.email, id);
        await ApplyProfile(entity, model);
        entity.email = model.email.Trim().ToLowerInvariant();
        entity.updated_at = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return await single_user(id);
    }

    [HttpPut]
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<User> update_user(int? id, [FromBody] User User)
    {
        await ValidateEmail(User.email, id);
        var requestedType = User.type ?? (User.role is 8 or 9 ? UserType.ADMIN : UserType.MEMBER);
        if (!Enum.IsDefined(requestedType))
            throw new ArgumentException("Invalid user type.");
        if (id == null && (User.password?.Length ?? 0) < 12)
            throw new ArgumentException("A password of at least 12 characters is required.");
        user user;

        if (id == null)
        {
            user = new user
            {
                created_at = DateTimeOffset.UtcNow,
                type = requestedType
            };

            db.users.Add(user);
        }
        else
        {
            user = await db.users
                .SingleAsync(a => a.id == id);

            user.updated_at = DateTimeOffset.UtcNow;

            if (User.type != null || User.role != null)
            {
                user.type = requestedType;
            }
        }

        await ApplyProfile(user, User);
        user.email = User.email.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(User.password))
        {
            if (User.password.Length < 12)
                throw new ArgumentException("Password must contain at least 12 characters.");
            user.password = this.passwordProvider.Generate(User.password).Hashed;
            user.password_set_at = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync();

        return await single_user(user.id);
    }

    private async Task ApplyProfile(user entity, User model)
    {
        entity.first_name = model.first_name ?? model.name;
        entity.last_name = model.last_name ?? model.surname;
        entity.address = model.address;
        entity.institution = model.institution;
        entity.faculty = model.faculty;
        entity.department = model.department;
        entity.iletisim = model.iletisim;
        entity.u_title = model.utitle?.id ?? model.u_title;
        entity.u_type = model.utype?.id ?? model.u_type;
        entity.gender = model.ugender?.id ?? model.gender;
        foreach (var (id, type) in new[] { (entity.u_title, "titlecon"), (entity.u_type, "materialcon"), (entity.gender, "gender") })
            if (id != null && !await db.Set<parameters>().AnyAsync(a => a.id == id && a.type == type))
                throw new ArgumentException("Invalid profile parameter: " + type);
        if (new[] { entity.first_name, entity.last_name, entity.institution, entity.faculty, entity.department, entity.iletisim }
            .Any(a => a?.Length > 256) || entity.address?.Length > 10000)
            throw new ArgumentException("Profile field is too long.");
    }

    private async Task ValidateEmail(string? email, int? id)
    {
        if (string.IsNullOrWhiteSpace(email) || !System.Net.Mail.MailAddress.TryCreate(email, out var address) || address.Address != email.Trim())
            throw new ArgumentException("A valid email address is required.");
        var normalized = email.Trim().ToLowerInvariant();
        if (await db.users.AnyAsync(a => a.email == normalized && a.id != id))
            throw new ArgumentException("This email address is already registered.");
    }
}
