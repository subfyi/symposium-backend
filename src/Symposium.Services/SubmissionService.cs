using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Services;

public sealed class SubmissionPermissionException : Exception
{
    public SubmissionPermissionException() : base("You cannot modify this submission.") { }
}

public sealed class SubmissionLockedException : Exception
{
    public SubmissionLockedException() : base("Approved or deleted submissions cannot be edited by members.") { }
}

public sealed class SubmissionService(AppDbContext db, SubmissionNotificationService notifications)
{
    public async Task<int> Save(int? id, Articles model, int actorId, bool admin)
    {
        var entity = id == null ? new articles { created_at = DateTimeOffset.UtcNow, pap_status = "0", title_year = DateTimeOffset.UtcNow.Year.ToString() }
            : await db.Set<articles>().SingleOrDefaultAsync(a => a.id == id) ?? throw new KeyNotFoundException();
        if (!admin && id != null && entity.pap_creater != actorId.ToString()) throw new SubmissionPermissionException();
        if (!admin && (entity.paper_approved == true || entity.pap_status == "3")) throw new SubmissionLockedException();
        if (string.IsNullOrWhiteSpace(model.en_title) || model.en_title.Length > 10000 || model.en_abstract?.Length > 200000)
            throw new ArgumentException("A title is required and text must fit the submission limits.");
        if (!admin && (string.IsNullOrWhiteSpace(model.en_abstract) || model.authors is not { Count: > 0 }))
            throw new ArgumentException("An abstract and at least one author are required.");
        var rows = id == null ? [] : await db.Set<authors>().Where(a => a.p_id == id).ToListAsync();
        if (model.authors != null)
        {
            var ids = model.authors.Where(a => a.id > 0).Select(a => a.id!.Value).ToList();
            if (model.authors.Count > 100 || ids.Count != ids.Distinct().Count() || model.authors.Any(a => a.id < 0) ||
                ids.Any(authorId => rows.All(a => a.id != authorId))) throw new ArgumentException("Invalid author IDs.");
            if (model.authors.Any(a => string.IsNullOrWhiteSpace(a.first_name) || string.IsNullOrWhiteSpace(a.last_name) ||
                !MailAddress.TryCreate(a.email, out var address) || address.Address != a.email?.Trim() ||
                a.first_name.Length > 256 || a.last_name.Length > 256 || a.adress?.Length > 10000))
                throw new ArgumentException("Each author needs a valid name and email address.");
            if (!admin && (model.authors.Count(a => a.correspond == true) != 1 || model.authors.Count(a => a.presenter == true) != 1))
                throw new ArgumentException("Choose exactly one corresponding author and presenter.");
        }
        await using var tx = db.Database.IsRelational() ? await db.Database.BeginTransactionAsync() : null;
        var wasApproved = entity.paper_approved == true;
        if (id == null) db.Add(entity);
        entity.en_title = model.en_title.Trim();
        entity.en_abstract = model.en_abstract;
        entity.pap_keyword = string.Join('|', (model.pap_keyword ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase).Take(100));
        entity.pap_topic_p = model.topic_specified ? model.topic?.id.ToString() : model.pap_topic_p;
        entity.pre_type = model.parampre_specified ? model.parampre?.id.ToString() : model.pre_type;
        entity.pap_type = model.parampap_specified ? model.parampap?.id.ToString() : model.pap_type;
        entity.mes_congress = model.mes_congress;
        if (entity.mes_congress?.Length > 10000) throw new ArgumentException("Message is too long.");
        entity.slug ??= Slug(entity.en_title) + "-" + Guid.NewGuid().ToString("N")[..8];
        entity.pap_creater = admin ? (model.user_specified ? model.user?.id?.ToString() : model.pap_creater ?? entity.pap_creater ?? actorId.ToString()) : actorId.ToString();
        if (!int.TryParse(entity.pap_creater, out var owner) || !await db.users.AnyAsync(a => a.id == owner))
            throw new ArgumentException("A valid submission owner is required.");
        entity.abstract_dosya_id = await Attachment(model.abstract_dosya, model.abstract_specified, model.abstract_dosya_id, entity.abstract_dosya_id, actorId, admin);
        entity.full_paper_dosya_id = await Attachment(model.full_paper_dosya, model.full_paper_specified, model.full_paper_dosya_id, entity.full_paper_dosya_id, actorId, admin);
        entity.poster_presentation_id = await Attachment(model.poster_presentation_dosya, model.poster_specified, model.poster_presentation_id, entity.poster_presentation_id, actorId, admin);
        entity.video_id = await Attachment(model.video, model.video_specified, model.video_id, entity.video_id, actorId, admin);
        if (admin)
        {
            entity.paper_approved = model.paper_approved ?? entity.paper_approved;
            entity.video_approved = model.video_approved ?? entity.video_approved;
            entity.title_year = model.title_year ?? entity.title_year;
            entity.pap_status = model.pap_status ?? entity.pap_status;
            entity.pap_payment = model.pap_payment ?? entity.pap_payment;
            entity.paper_page = model.paper_page;
            entity.paper_page_end = model.paper_page_end;
            entity.paper_abst_page = model.paper_abst_page;
            entity.pap_num = model.pap_num;
            entity.paper_external = model.paper_external;
            if (entity.paper_external != null && (!Uri.TryCreate(entity.paper_external, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https")))
                throw new ArgumentException("External paper URLs must use HTTP or HTTPS.");
        }
        else if (id == null) { entity.paper_approved = false; entity.video_approved = false; }
        foreach (var (value, type) in new[] { (entity.pap_topic_p, "keywordcon"), (entity.pre_type, "uygulamacon"), (entity.pap_type, "birimcon") })
            if (!admin && value != null && !await db.Set<parameters>().AnyAsync(a => a.id.ToString() == value && a.type == type))
                throw new ArgumentException("Invalid submission parameter.");
        entity.updated_at = id == null ? null : DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        if (model.authors != null)
        {
            db.RemoveRange(rows.Where(a => !model.authors.Any(b => b.id == a.id)));
            var index = 0;
            foreach (var author in model.authors)
            {
                var row = rows.SingleOrDefault(a => a.id == author.id) ?? new authors { p_id = entity.id, created_at = DateTimeOffset.UtcNow };
                if (row.id == 0) db.Add(row);
                row.sort_order = index++;
                row.p_user = actorId.ToString();
                row.first_name = author.first_name?.Trim(); row.last_name = author.last_name?.Trim();
                row.email = author.email?.Trim().ToLowerInvariant(); row.adress = author.adress;
                row.correspond = author.correspond == true ? "1" : "0";
                row.presenter = author.presenter == true ? "1" : "0";
                row.updated_at = DateTimeOffset.UtcNow;
            }
        }
        // Serialize keyword creation without requiring uniqueness on unrelated legacy parameters.
        if (db.Database.IsRelational())
            await db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(72401601)");
        foreach (var keyword in entity.pap_keyword.Split('|', StringSplitOptions.RemoveEmptyEntries))
        {
            if (keyword.Length > 256) throw new ArgumentException("Keywords must contain at most 256 characters.");
            if (!await db.Set<parameters>().AnyAsync(a => a.type == "tagcon" && a.value != null && a.value.ToLower() == keyword.ToLower()))
                db.Add(new parameters { type = "tagcon", value = keyword, item_num = "1", created_at = DateTimeOffset.UtcNow });
        }
        await db.SaveChangesAsync();
        if (!wasApproved && entity.paper_approved == true)
            await notifications.Queue(entity, "approved", actorId, "Your paper has been approved.");
        if (model.icerikler != null)
            await SyncComments(entity, model.icerikler, actorId, admin);
        await db.SaveChangesAsync();
        if (tx != null) await tx.CommitAsync();
        return entity.id;
    }

    private async Task<int?> Attachment(SubmissionFile? model, bool specified, int? scalarId, int? current, int actor, bool admin)
    {
        if (model?.storage_file_id is int storageId)
        {
            var file = await db.files.SingleOrDefaultAsync(a => a.id == storageId) ?? throw new ArgumentException("File does not exist.");
            if (!admin && file.owner_id != actor) throw new SubmissionPermissionException();
            var wrapper = await db.Set<dosyalar>().FirstOrDefaultAsync(a => a.storage_file_id == storageId);
            if (wrapper != null) return wrapper.id;
            wrapper = new dosyalar { storage_file_id = storageId, f_icerik_olusturan = actor.ToString(), r_dosyaisimi = file.name, f_dosya_boyut = file.size.ToString(), created_at = DateTimeOffset.UtcNow };
            db.Add(wrapper); await db.SaveChangesAsync(); return wrapper.id;
        }
        var id = specified ? model?.id : scalarId ?? current;
        if (id == null || id == current) return id;
        var legacy = await db.Set<dosyalar>().SingleOrDefaultAsync(a => a.id == id) ?? throw new ArgumentException("Attachment does not exist.");
        if (!admin && legacy.f_icerik_olusturan != actor.ToString()) throw new SubmissionPermissionException();
        return id;
    }

    public async Task<int> AddComment(int id, string? body, int actorId, bool admin)
    {
        var entity = await db.Set<articles>().SingleOrDefaultAsync(a => a.id == id) ?? throw new KeyNotFoundException();
        if (!admin && (entity.pap_creater != actorId.ToString() || entity.pap_status == "3")) throw new SubmissionPermissionException();
        ValidateComment(body);
        await using var tx = db.Database.IsRelational() ? await db.Database.BeginTransactionAsync() : null;
        var comment = new submission_comment { article_id = id, author_id = actorId, body = body!.Trim(), created_at = DateTimeOffset.UtcNow };
        db.Add(comment);
        await notifications.Queue(entity, "comment", actorId, comment.body);
        await db.SaveChangesAsync();
        if (tx != null) await tx.CommitAsync();
        return comment.id;
    }

    private async Task SyncComments(articles entity, IReadOnlyCollection<SubmissionComment> models, int actor, bool admin)
    {
        var rows = await db.Set<submission_comment>().Where(a => a.article_id == entity.id).ToListAsync();
        var ids = models.Where(a => a.id > 0).Select(a => a.id!.Value).ToList();
        if (ids.Count != ids.Distinct().Count() || ids.Any(id => rows.All(a => a.id != id)))
            throw new ArgumentException("Invalid comment IDs.");
        // Omitted comments are preserved so a stale submission form cannot delete newer replies.
        foreach (var model in models)
        {
            ValidateComment(model.isicerik);
            var row = rows.SingleOrDefault(a => a.id == model.id);
            if (row == null)
            {
                db.Add(new submission_comment { article_id = entity.id, author_id = actor, body = model.isicerik!.Trim(), created_at = DateTimeOffset.UtcNow });
                await notifications.Queue(entity, "comment", actor, model.isicerik!.Trim());
            }
            else if (row.body != model.isicerik)
            {
                if (!admin && row.author_id != actor) throw new SubmissionPermissionException();
                row.body = model.isicerik!; row.updated_at = DateTimeOffset.UtcNow;
            }
        }
    }

    private static void ValidateComment(string? body)
    {
        if (string.IsNullOrWhiteSpace(body) || body.Length > 10000) throw new ArgumentException("Comments must contain between 1 and 10000 characters.");
    }

    public async Task ChangeComment(int articleId, int commentId, string? body, int actor, bool admin, bool delete = false)
    {
        var article = await db.Set<articles>().SingleOrDefaultAsync(a => a.id == articleId) ?? throw new KeyNotFoundException();
        if (!admin && (article.pap_creater != actor.ToString() || article.pap_status == "3")) throw new SubmissionPermissionException();
        var row = await db.Set<submission_comment>().SingleOrDefaultAsync(a => a.id == commentId && a.article_id == articleId)
            ?? throw new KeyNotFoundException();
        if (!admin && row.author_id != actor) throw new SubmissionPermissionException();
        if (delete) db.Remove(row);
        else { ValidateComment(body); row.body = body!.Trim(); row.updated_at = DateTimeOffset.UtcNow; }
        await db.SaveChangesAsync();
    }

    private static string Slug(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var ascii = new string(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        return Regex.Replace(ascii.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-')[..Math.Min(120, Regex.Replace(ascii.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-').Length)];
    }
}
