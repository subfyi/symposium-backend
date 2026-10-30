using Microsoft.EntityFrameworkCore;
using Symposium.Storage;
using Xunit;

namespace Symposium.Tests;

public class SchemaTests
{
    private static AppDbContext Database() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql("Host=127.0.0.1;Port=1;Database=unused;Username=unused").Options);

    [Theory]
    [InlineData("en_title")]
    [InlineData("en_abstract")]
    [InlineData("paper_external")]
    public void LongArticleFieldsAreNotLimitedTo256Characters(string field)
    {
        using var db = Database();
        var property = db.Model.FindEntityType(typeof(articles))!.FindProperty(field)!;
        Assert.Equal("text", property.GetColumnType());
        Assert.Null(property.GetMaxLength());
    }

    [Fact]
    public void EmailHasDatabaseEnforcedNormalizedUniqueness()
    {
        using var db = Database();
        var entity = db.Model.FindEntityType(typeof(user))!;
        Assert.Equal("lower(btrim(email))", entity.FindProperty("normalized_email")!.GetComputedColumnSql());
        Assert.Contains(entity.GetIndexes(), a => a.IsUnique && a.Properties.Single().Name == "normalized_email");
    }

    [Fact]
    public void LegacySubmissionAttachmentsUseDosyalarInsteadOfNewFiles()
    {
        using var db = Database();
        var entity = db.Model.FindEntityType(typeof(articles))!;
        foreach (var field in new[] { "abstract_dosya_id", "full_paper_dosya_id", "poster_presentation_id", "video_id" })
            Assert.Contains(entity.GetForeignKeys(), a => a.Properties.Single().Name == field && a.PrincipalEntityType.ClrType == typeof(dosyalar));
    }
}
