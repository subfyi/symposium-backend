using JustAuthenticator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Tamturk;
using Tamturk.AspNetCore;

namespace Symposium.Api.Server.Controllers;

public class RegisterModel
{
    [Required, MaxLength(256)]
    public string name { get; set; }

    public string? surname { get; set; }

    [Required, EmailAddress, MaxLength(256)]
    public string email { get; set; }

    [Required, MinLength(12), MaxLength(256)]
    public string password { get; set; }
}

public class PasswordUpdateModel
{
    public string current_password { get; set; }

    public string new_password { get; set; }
}

[Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("authentication")]
public class AccountController : ControllerBase
{
    private readonly AppDbContext db;
    private readonly IPasswordProvider passwordProvider;

    public AccountController(AppDbContext db, IPasswordProvider passwordProvider)
    {
        this.db = db;
        this.passwordProvider = passwordProvider;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel body, [FromServices] IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("registration:enabled"))
            return NotFound();
        if (!TryValidateModel(body))
            return ValidationProblem(ModelState);
        var email = body.email.Trim().ToLowerInvariant();
        if (await db.users.AnyAsync(a => a.email == email))
            return Conflict(new { message = "This email address is already registered." });
        db.users.Add(new user
        {
            first_name = body.name.Trim(),
            last_name = body.surname?.Trim(),
            email = email,
            password = passwordProvider.Generate(body.password).Hashed,
            password_set_at = DateTimeOffset.UtcNow,
            created_at = DateTimeOffset.UtcNow,
            type = UserType.MEMBER
        });
        await db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("user/me/password")]
    [Authorize]
    public async Task UpdatePassword([FromBody] PasswordUpdateModel request)
    {
        if (string.IsNullOrEmpty(request.current_password) || (request.new_password?.Length ?? 0) < 12 || request.new_password?.Length > 256)
            throw new ArgumentException("Password must contain at least 12 characters.");
        int user_id = int.Parse(this.User.FindFirst("user_id").Value);
        var user = await db.users.Where(a => a.id == user_id).SingleAsync();

        if (!string.IsNullOrEmpty(user.password) && !passwordProvider
                .Generate(request.current_password)
                .Compare(user.password))
        {
            throw new ArgumentException("PASSWORD.INVALID");
        }

        user.password = passwordProvider
            .Generate(request.new_password)
            .Hashed;
        user.password_set_at = DateTimeOffset.UtcNow;

        db.tokens.RemoveRange(await db.tokens.Where(a => a.user_id == user_id).ToListAsync());

        await db.SaveChangesAsync();
    }

    public async Task SendCode(string to, string name, int code)
    {
        var from = Request.HttpContext.RequestServices.GetRequiredService<MailAddress>();

        using MailMessage msg = new(from, new MailAddress(to, name, Encoding.UTF8))
        {
            Subject = $"Confirm your e-mail address: {code}"
        };

        using AlternateView textView =
            AlternateView.CreateAlternateViewFromString($@"Hello {name},

Please confirm your e-mail address with this code: {code}

Best regards,
Symposium");
        textView.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;

        textView.ContentType = new System.Net.Mime.ContentType("text/plain; charset=UTF-8");
        msg.AlternateViews.Add(textView);

        using AlternateView htmlView = AlternateView.CreateAlternateViewFromString($@"<p>Hello <b>{System.Net.WebUtility.HtmlEncode(name)}</b>,</p>

<p>Please confirm your e-mail address with this code: {code}</p>

<p>Best regards,<br/>
Symposium</p>");
        htmlView.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
        htmlView.ContentType = new System.Net.Mime.ContentType("text/html; charset=UTF-8");

        msg.AlternateViews.Add(htmlView);

        await Request.HttpContext.RequestServices.GetRequiredService<SmtpClient>().SendMailAsync(msg);
    }

    [HttpGet("reset")]
    public IActionResult Reset()
    {
        Request.ValidateRequest();
        return NoContent();
    }

    [HttpPost("reset")]
    public async Task<string> Reset([FromBody] RegisterModel body, [FromQuery] string email,
        [FromServices] IRequestSigning ss)
    {
        if (string.IsNullOrWhiteSpace(body.email))
            throw new ArgumentException("Email is required.");
        body.email = body.email.Trim().ToLowerInvariant();

        user user = await db.users.FirstOrDefaultAsync(a => a.email == body.email);
        if (user == null)
        {
            throw new ArgumentException("Unable to reset this account.");
        }

        if (email != null)
        {
            if (!Request.TryValidateRequest() || email != body.email)
            {
                throw new ArgumentException("Invalid verification code: Please try again.");
            }

            if ((body.password?.Length ?? 0) < 12)
                throw new ArgumentException("Password must contain at least 12 characters.");
            user.password = passwordProvider.Generate(body.password).Hashed;
            user.password_set_at = DateTimeOffset.UtcNow;
            db.tokens.RemoveRange(await db.tokens.Where(a => a.user_id == user.id).ToListAsync());

            await db.SaveChangesAsync();
            return "Ok";
        }

        int code = RandomNumberGenerator.GetInt32(100000, 1000000);

        var signature = ss.SignRequest("GET,POST", "/reset", new Dictionary<string, string>
        {
            {"email", body.email}
        }, DateTimeOffset.UtcNow.AddHours(1), new Dictionary<string, string>
        {
            {"code", code.ToString()}
        });

        await SendCode(body.email, $"{user.first_name} {user.last_name}", code);

        return signature;
    }
}
