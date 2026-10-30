using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Symposium.Storage;

public class submission_comment
{
    public int id { get; set; }
    public int article_id { get; set; }
    public int author_id { get; set; }
    public string body { get; set; } = "";
    public DateTimeOffset created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class submission_notification
{
    public int id { get; set; }
    public int article_id { get; set; }
    public int? user_id { get; set; }
    public string kind { get; set; } = "";
    public string recipient { get; set; } = "";
    public string subject { get; set; } = "";
    public string body { get; set; } = "";
    public DateTimeOffset created_at { get; set; }
    public DateTimeOffset? read_at { get; set; }
    public DateTimeOffset? sent_at { get; set; }
    public DateTimeOffset next_attempt_at { get; set; }
    public DateTimeOffset? claimed_until { get; set; }
    public int attempts { get; set; }
}

public class submission_language
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public string code { get; set; } = "";
    public bool active { get; set; }
    public bool rtl { get; set; }
}

public class SubmissionCommentConfiguration : IEntityTypeConfiguration<submission_comment>
{
    public void Configure(EntityTypeBuilder<submission_comment> b)
    {
        b.ToTable("submission_comments");
        b.HasKey(a => a.id);
        b.Property(a => a.body).HasColumnType("text").Metadata.SetMaxLength(null);
        b.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<user>().WithMany().HasForeignKey(a => a.author_id).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(a => new { a.article_id, a.created_at });
    }
}

public class SubmissionNotificationConfiguration : IEntityTypeConfiguration<submission_notification>
{
    public void Configure(EntityTypeBuilder<submission_notification> b)
    {
        b.ToTable("submission_notifications");
        b.HasKey(a => a.id);
        b.Property(a => a.body).HasColumnType("text").Metadata.SetMaxLength(null);
        b.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<user>().WithMany().HasForeignKey(a => a.user_id).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(a => new { a.sent_at, a.next_attempt_at });
    }
}

public class SubmissionLanguageConfiguration : IEntityTypeConfiguration<submission_language>
{
    public void Configure(EntityTypeBuilder<submission_language> b)
    {
        b.ToTable("submission_languages");
        b.HasKey(a => a.id);
        b.HasIndex(a => a.code).IsUnique();
    }
}

public class SubmissionFileConfiguration : IEntityTypeConfiguration<dosyalar>
{
    public void Configure(EntityTypeBuilder<dosyalar> b) => b.HasOne(a => a.storage_file).WithMany()
        .HasForeignKey(a => a.storage_file_id).OnDelete(DeleteBehavior.Restrict);
}

public class OwnedFileConfiguration : IEntityTypeConfiguration<vfile>
{
    public void Configure(EntityTypeBuilder<vfile> b) => b.HasOne<user>().WithMany()
        .HasForeignKey(a => a.owner_id).OnDelete(DeleteBehavior.SetNull);
}

public class UserProfileConfiguration : IEntityTypeConfiguration<user>
{
    public void Configure(EntityTypeBuilder<user> b) => b.Property(a => a.address)
        .HasColumnType("text").Metadata.SetMaxLength(null);
}
