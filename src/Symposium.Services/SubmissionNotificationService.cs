using Microsoft.EntityFrameworkCore;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Services;

public sealed class SubmissionNotificationService(AppDbContext db)
{
    public async Task Queue(articles article, string kind, int actor, string text)
    {
        var emails = await db.Set<authors>().Where(a => a.p_id == article.id && a.correspond == "1" && a.email != null)
            .Select(a => a.email!).ToListAsync();
        var owner = await db.users.FirstOrDefaultAsync(a => a.id.ToString() == article.pap_creater);
        if (owner != null) emails.Add(owner.email);
        if (kind == "comment")
        {
            emails.AddRange(await db.users.Where(a => a.type == UserType.ADMIN).Select(a => a.email).ToListAsync());
            var sender = await db.users.Where(a => a.id == actor).Select(a => a.email).SingleAsync();
            emails.RemoveAll(a => string.Equals(a, sender, StringComparison.OrdinalIgnoreCase));
        }
        foreach (var email in emails.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!System.Net.Mail.MailAddress.TryCreate(email, out _)) continue;
            var userId = await db.users.Where(a => a.email.ToLower() == email.ToLower()).Select(a => (int?)a.id).FirstOrDefaultAsync();
            db.Add(new submission_notification
            {
                article_id = article.id, user_id = userId, kind = kind, recipient = email,
                subject = kind == "approved" ? "Paper approved" : "New paper comment",
                body = $"Paper #{article.id}: {article.en_title}\n\n{text}",
                created_at = DateTimeOffset.UtcNow, next_attempt_at = DateTimeOffset.UtcNow
            });
        }
    }
}
