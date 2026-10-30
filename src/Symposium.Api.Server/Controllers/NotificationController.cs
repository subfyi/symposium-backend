using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Symposium.Api.Server.Controllers;

[Authorize, Route("notification")]
public class NotificationController(AppDbContext db) : ControllerBase
{
    private int Actor => int.Parse(User.FindFirstValue("user_id")!);

    [HttpGet]
    public Task<Pagination<object>> All() => this.Paginate(db.Set<submission_notification>().Where(a => a.user_id == Actor)
        .OrderByDescending(a => a.id), q => q.Select(a => (object)new { a.id, a.article_id, a.kind, a.subject, a.body, a.created_at, a.read_at }));

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> Read(int id)
    {
        var updated = await db.Set<submission_notification>().Where(a => a.id == id && a.user_id == Actor)
            .ExecuteUpdateAsync(a => a.SetProperty(b => b.read_at, DateTimeOffset.UtcNow));
        return updated == 0 ? NotFound() : NoContent();
    }

    [HttpGet("outbox"), Authorize(Roles = "Admin")]
    public Task<Pagination<object>> Outbox() => this.Paginate(db.Set<submission_notification>().OrderByDescending(a => a.id),
        q => q.Select(a => (object)new { a.id, a.article_id, a.kind, a.sent_at, a.attempts, a.next_attempt_at, a.claimed_until }));
}
