using System.Net.Mail;

namespace Symposium.Api.Server.Services;

public sealed class NotificationDispatcher(IServiceScopeFactory scopes, IConfiguration configuration, ILogger<NotificationDispatcher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue<bool>("notifications:dispatchEnabled", true)) return;
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await Dispatch(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception e) { logger.LogError(e, "Notification dispatch failed; queued messages are retained."); }
            try { await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, configuration.GetValue<int>("notifications:intervalSeconds", 15))), stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task Dispatch(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var ids = await db.Set<submission_notification>().Where(a => a.sent_at == null && a.next_attempt_at <= now &&
            (a.claimed_until == null || a.claimed_until < now)).OrderBy(a => a.id).Select(a => a.id).Take(20).ToListAsync(ct);
        foreach (var id in ids)
        {
            // Atomic leases avoid simultaneous delivery from multiple API processes.
            var claimed = await db.Set<submission_notification>().Where(a => a.id == id && a.sent_at == null &&
                (a.claimed_until == null || a.claimed_until < now))
                .ExecuteUpdateAsync(a => a.SetProperty(b => b.claimed_until, now.AddMinutes(5)), ct);
            if (claimed == 0) continue;
            var row = await db.Set<submission_notification>().SingleAsync(a => a.id == id, ct);
            try
            {
                using var message = new MailMessage(scope.ServiceProvider.GetRequiredService<MailAddress>(), new MailAddress(row.recipient))
                { Subject = row.subject, Body = row.body, IsBodyHtml = false };
                await scope.ServiceProvider.GetRequiredService<SmtpClient>().SendMailAsync(message, ct);
                row.sent_at = DateTimeOffset.UtcNow;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
            catch (Exception e)
            {
                logger.LogWarning(e, "Notification {Id} will be retried.", id);
                row.next_attempt_at = DateTimeOffset.UtcNow.AddSeconds(Math.Min(21600,
                    Math.Max(1, configuration.GetValue<int>("notifications:retryBaseSeconds", 60)) * Math.Pow(2, Math.Min(row.attempts, 9))));
            }
            row.attempts++;
            row.claimed_until = null;
            await db.SaveChangesAsync(ct);
        }
    }
}
