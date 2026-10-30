using Microsoft.EntityFrameworkCore;
using Symposium.Models;
using Symposium.Services;
using Symposium.Storage;
using Xunit;

namespace Symposium.Tests;

public class SubmissionWorkflowTests
{
    private static async Task<AppDbContext> Database()
    {
        var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        db.users.AddRange(new user { id = 1, email = "owner@example.test", password = "test", type = UserType.MEMBER },
            new user { id = 2, email = "other@example.test", password = "test", type = UserType.MEMBER },
            new user { id = 3, email = "admin@example.test", password = "test", type = UserType.ADMIN });
        await db.SaveChangesAsync(); return db;
    }
    private static Articles Payload() => new()
    {
        en_title = "A paper", en_abstract = "Abstract", pap_keyword = "Ecology| ecology |Climate",
        paper_approved = true, video_approved = true, pap_payment = "forged", pap_creater = "2", title_year = "1999",
        authors = [new() { first_name = "One", last_name = "Author", email = "one@example.test", correspond = true, presenter = true },
            new() { first_name = "Two", last_name = "Author", email = "two@example.test" }]
    };
    private static SubmissionService Service(AppDbContext db) => new(db, new SubmissionNotificationService(db));

    [Fact]
    public async Task MemberCanCreateAndEditOwnDraftButCannotSetPrivilegedFields()
    {
        await using var db = await Database();
        var service = Service(db);
        var id = await service.Save(null, Payload(), 1, false);
        var row = await db.Set<articles>().SingleAsync();
        Assert.Equal("1", row.pap_creater); Assert.False(row.paper_approved); Assert.False(row.video_approved);
        Assert.Null(row.pap_payment); Assert.NotEqual("1999", row.title_year);
        await service.Save(id, Payload() with { en_title = "Updated" }, 1, false);
        Assert.Equal("Updated", row.en_title);
    }

    [Fact]
    public async Task OtherMembersCannotModifyOrCommentOnDraft()
    {
        await using var db = await Database();
        var service = Service(db); var id = await service.Save(null, Payload(), 1, false);
        await Assert.ThrowsAsync<SubmissionPermissionException>(() => service.Save(id, Payload(), 2, false));
        await Assert.ThrowsAsync<SubmissionPermissionException>(() => service.AddComment(id, "Intrusion", 2, false));
        Assert.Empty(db.Set<submission_comment>());
    }

    [Fact]
    public async Task ApprovalQueuesMailOnceAndLocksMemberEditsButAllowsComments()
    {
        await using var db = await Database();
        var service = Service(db); var id = await service.Save(null, Payload(), 1, false);
        var adminPayload = Payload() with { pap_creater = "1", authors = null };
        await service.Save(id, adminPayload, 3, true);
        var count = await db.Set<submission_notification>().CountAsync(a => a.kind == "approved");
        Assert.True(count > 0);
        await service.Save(id, adminPayload, 3, true);
        Assert.Equal(count, await db.Set<submission_notification>().CountAsync(a => a.kind == "approved"));
        await Assert.ThrowsAsync<SubmissionLockedException>(() => service.Save(id, Payload(), 1, false));
        await service.AddComment(id, "Question", 1, false);
        Assert.Single(db.Set<submission_comment>());
        Assert.Contains(db.Set<submission_notification>(), a => a.recipient == "admin@example.test" && a.kind == "comment");
    }

    [Fact]
    public async Task AuthorOrderPersistsAndKeywordsAreDeduplicated()
    {
        await using var db = await Database();
        var service = Service(db); var id = await service.Save(null, Payload(), 1, false);
        var rows = await db.Set<authors>().OrderBy(a => a.sort_order).ToListAsync();
        Assert.Equal(2, await db.Set<parameters>().CountAsync(a => a.type == "tagcon"));
        await service.Save(id, Payload() with { authors = rows.AsEnumerable().Reverse().Select(a => new SubmissionAuthor
        { id = a.id, first_name = a.first_name, last_name = a.last_name, email = a.email, correspond = a.correspond == "1", presenter = a.presenter == "1" }).ToList() }, 1, false);
        Assert.Equal("Two", (await db.Set<authors>().OrderBy(a => a.sort_order).FirstAsync()).first_name);
    }

    [Fact]
    public async Task MemberCannotAttachSomeoneElsesUploadedFile()
    {
        await using var db = await Database();
        db.files.Add(new vfile { id = 10, owner_id = 2, folder_id = "test", name = "private.pdf", content_type = "application/pdf", access_url = "https://example.test/private.pdf" });
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<SubmissionPermissionException>(() => Service(db).Save(null,
            Payload() with { abstract_dosya = new SubmissionFile { storage_file_id = 10 } }, 1, false));
    }

    [Fact]
    public async Task StaleSubmissionCannotDeleteNewCommentsOrEditModeratorReply()
    {
        await using var db = await Database();
        var service = Service(db); var id = await service.Save(null, Payload(), 1, false);
        await service.AddComment(id, "Question", 1, false);
        var reply = await service.AddComment(id, "Admin reply", 3, true);
        await service.Save(id, Payload() with { icerikler = [] }, 1, false);
        Assert.Equal(2, await db.Set<submission_comment>().CountAsync());
        await Assert.ThrowsAsync<SubmissionPermissionException>(() => service.ChangeComment(id, reply, "Forged", 1, false));
    }

    [Theory]
    [InlineData("/folder/paper.pdf", "https://cdn.example.test/folder/paper.pdf")]
    [InlineData("https://cdn.example.test/paper.pdf", "https://cdn.example.test/paper.pdf")]
    public void FileUrlsArePrefixedExactlyOnce(string input, string expected)
    {
        var options = new System.Text.Json.JsonSerializerOptions();
        options.Converters.Add(new Symposium.Api.Server.Converters.FileConverter("https://cdn.example.test/"));
        var json = System.Text.Json.JsonSerializer.Serialize(new Symposium.Models.File { id = 1, access_url = input }, options);
        Assert.Contains(expected, json);
    }
}
