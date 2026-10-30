using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Symposium.Storage;

public class article_info_configuration : IEntityTypeConfiguration<article_info>
{
    public void Configure(EntityTypeBuilder<article_info> builder)
    {
        builder.ToTable("article_info");
        builder.Property(a => a.title).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.@abstract).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<volumes>().WithMany().HasForeignKey(a => a.v_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_to_article_type_configuration : IEntityTypeConfiguration<article_to_article_type>
{
    public void Configure(EntityTypeBuilder<article_to_article_type> builder)
    {
        builder.ToTable("article_to_article_type");
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<article_types>().WithMany().HasForeignKey(a => a.article_type_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_to_author_configuration : IEntityTypeConfiguration<article_to_author>
{
    public void Configure(EntityTypeBuilder<article_to_author> builder)
    {
        builder.ToTable("article_to_author");
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<authors>().WithMany().HasForeignKey(a => a.author_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_to_citation_configuration : IEntityTypeConfiguration<article_to_citation>
{
    public void Configure(EntityTypeBuilder<article_to_citation> builder)
    {
        builder.ToTable("article_to_citation");
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<citations>().WithMany().HasForeignKey(a => a.citation_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_to_files_configuration : IEntityTypeConfiguration<article_to_files>
{
    public void Configure(EntityTypeBuilder<article_to_files> builder)
    {
        builder.ToTable("article_to_files");
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<vfile>().WithMany().HasForeignKey(a => a.file_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_to_keyword_configuration : IEntityTypeConfiguration<article_to_keyword>
{
    public void Configure(EntityTypeBuilder<article_to_keyword> builder)
    {
        builder.ToTable("article_to_keyword");
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<keywords>().WithMany().HasForeignKey(a => a.keyword_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_to_subject_configuration : IEntityTypeConfiguration<article_to_subject>
{
    public void Configure(EntityTypeBuilder<article_to_subject> builder)
    {
        builder.ToTable("article_to_subject");
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<subjects>().WithMany().HasForeignKey(a => a.subject_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class article_types_configuration : IEntityTypeConfiguration<article_types>
{
    public void Configure(EntityTypeBuilder<article_types> builder)
    {
        builder.ToTable("article_types");
    }
}

public class articles_configuration : IEntityTypeConfiguration<articles>
{
    public void Configure(EntityTypeBuilder<articles> builder)
    {
        builder.ToTable("articles");
        builder.Property(a => a.en_title).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.en_abstract).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.pap_keyword).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.mes_congress).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.paper_external).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.HasOne<dosyalar>().WithMany().HasForeignKey(a => a.abstract_dosya_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<dosyalar>().WithMany().HasForeignKey(a => a.full_paper_dosya_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<dosyalar>().WithMany().HasForeignKey(a => a.poster_presentation_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<dosyalar>().WithMany().HasForeignKey(a => a.video_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class authors_configuration : IEntityTypeConfiguration<authors>
{
    public void Configure(EntityTypeBuilder<authors> builder)
    {
        builder.ToTable("authors");
        builder.Property(a => a.adress).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.p_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class citations_configuration : IEntityTypeConfiguration<citations>
{
    public void Configure(EntityTypeBuilder<citations> builder)
    {
        builder.ToTable("citations");
        builder.Property(a => a.raw).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.HasOne<articles>().WithMany().HasForeignKey(a => a.article_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class countries_configuration : IEntityTypeConfiguration<countries>
{
    public void Configure(EntityTypeBuilder<countries> builder)
    {
        builder.ToTable("countries");
        builder.Property(a => a.provinces).HasColumnType("text").Metadata.SetMaxLength(null);
    }
}

public class dosyalar_configuration : IEntityTypeConfiguration<dosyalar>
{
    public void Configure(EntityTypeBuilder<dosyalar> builder)
    {
        builder.ToTable("dosyalar");
    }
}

public class feedbacks_configuration : IEntityTypeConfiguration<feedbacks>
{
    public void Configure(EntityTypeBuilder<feedbacks> builder)
    {
        builder.ToTable("feedbacks");
    }
}

public class feedbackdata_configuration : IEntityTypeConfiguration<feedbackdata>
{
    public void Configure(EntityTypeBuilder<feedbackdata> builder)
    {
        builder.ToTable("feedbackdata");
        builder.Property(a => a.isicerik).HasColumnType("text").Metadata.SetMaxLength(null);
    }
}

public class gorev_files_configuration : IEntityTypeConfiguration<gorev_files>
{
    public void Configure(EntityTypeBuilder<gorev_files> builder)
    {
        builder.ToTable("gorev_files");
    }
}

public class gorev_icerik_configuration : IEntityTypeConfiguration<gorev_icerik>
{
    public void Configure(EntityTypeBuilder<gorev_icerik> builder)
    {
        builder.ToTable("gorev_icerik");
        builder.Property(a => a.isicerik).HasColumnType("text").Metadata.SetMaxLength(null);
    }
}

public class journal_info_configuration : IEntityTypeConfiguration<journal_info>
{
    public void Configure(EntityTypeBuilder<journal_info> builder)
    {
        builder.ToTable("journal_info");
        foreach (var property in new[] { "title", "subtitle", "description", "title_abbr", "footer_text", "mail_signature", "aim", "scope", "writing_rules", "policy" })
            builder.Property<string>(property).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.HasOne<journals>().WithMany().HasForeignKey(a => a.journal_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class journal_to_article_type_configuration : IEntityTypeConfiguration<journal_to_article_type>
{
    public void Configure(EntityTypeBuilder<journal_to_article_type> builder)
    {
        builder.ToTable("journal_to_article_type");
        builder.HasOne<journals>().WithMany().HasForeignKey(a => a.journal_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<article_types>().WithMany().HasForeignKey(a => a.article_type_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class journal_to_countrie_configuration : IEntityTypeConfiguration<journal_to_countrie>
{
    public void Configure(EntityTypeBuilder<journal_to_countrie> builder)
    {
        builder.ToTable("journal_to_countrie");
        builder.HasOne<journals>().WithMany().HasForeignKey(a => a.journal_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<countries>().WithMany().HasForeignKey(a => a.countrie_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class journal_to_publisher_configuration : IEntityTypeConfiguration<journal_to_publisher>
{
    public void Configure(EntityTypeBuilder<journal_to_publisher> builder)
    {
        builder.ToTable("journal_to_publisher");
        builder.HasOne<journals>().WithMany().HasForeignKey(a => a.journal_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<publishers>().WithMany().HasForeignKey(a => a.publisher_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class journal_to_subject_configuration : IEntityTypeConfiguration<journal_to_subject>
{
    public void Configure(EntityTypeBuilder<journal_to_subject> builder)
    {
        builder.ToTable("journal_to_subject");
        builder.HasOne<journals>().WithMany().HasForeignKey(a => a.journal_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<subjects>().WithMany().HasForeignKey(a => a.subject_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class journals_configuration : IEntityTypeConfiguration<journals>
{
    public void Configure(EntityTypeBuilder<journals> builder)
    {
        builder.ToTable("journals");
        builder.Property(a => a.address).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.url).HasColumnType("text").Metadata.SetMaxLength(null);
    }
}

public class keywords_configuration : IEntityTypeConfiguration<keywords>
{
    public void Configure(EntityTypeBuilder<keywords> builder)
    {
        builder.ToTable("keywords");
    }
}

public class parameters_configuration : IEntityTypeConfiguration<parameters>
{
    public void Configure(EntityTypeBuilder<parameters> builder)
    {
        builder.ToTable("parameters");
        builder.Property(a => a.value).HasColumnType("text").Metadata.SetMaxLength(null);
    }
}

public class presentations_configuration : IEntityTypeConfiguration<presentations>
{
    public void Configure(EntityTypeBuilder<presentations> builder)
    {
        builder.ToTable("presentations");
    }
}

public class publishers_configuration : IEntityTypeConfiguration<publishers>
{
    public void Configure(EntityTypeBuilder<publishers> builder)
    {
        builder.ToTable("publishers");
    }
}

public class publishs_configuration : IEntityTypeConfiguration<publishs>
{
    public void Configure(EntityTypeBuilder<publishs> builder)
    {
        builder.ToTable("publishs");
    }
}

public class roles_configuration : IEntityTypeConfiguration<roles>
{
    public void Configure(EntityTypeBuilder<roles> builder)
    {
        builder.ToTable("roles");
    }
}

public class subjects_configuration : IEntityTypeConfiguration<subjects>
{
    public void Configure(EntityTypeBuilder<subjects> builder)
    {
        builder.ToTable("subjects");
    }
}

public class topics_configuration : IEntityTypeConfiguration<topics>
{
    public void Configure(EntityTypeBuilder<topics> builder)
    {
        builder.ToTable("topics");
    }
}

public class types_configuration : IEntityTypeConfiguration<types>
{
    public void Configure(EntityTypeBuilder<types> builder)
    {
        builder.ToTable("types");
    }
}

public class volume_to_files_configuration : IEntityTypeConfiguration<volume_to_files>
{
    public void Configure(EntityTypeBuilder<volume_to_files> builder)
    {
        builder.ToTable("volume_to_files");
        builder.Property(a => a.title).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.Property(a => a.description).HasColumnType("text").Metadata.SetMaxLength(null);
        builder.HasOne<volumes>().WithMany().HasForeignKey(a => a.volume_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<vfile>().WithMany().HasForeignKey(a => a.file_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class volume_to_year_configuration : IEntityTypeConfiguration<volume_to_year>
{
    public void Configure(EntityTypeBuilder<volume_to_year> builder)
    {
        builder.ToTable("volume_to_year");
    }
}

public class volumes_configuration : IEntityTypeConfiguration<volumes>
{
    public void Configure(EntityTypeBuilder<volumes> builder)
    {
        builder.ToTable("volumes");
        builder.HasOne<journals>().WithMany().HasForeignKey(a => a.journal_id).OnDelete(DeleteBehavior.Restrict);
    }
}

public class yorumlar_configuration : IEntityTypeConfiguration<yorumlar>
{
    public void Configure(EntityTypeBuilder<yorumlar> builder)
    {
        builder.ToTable("yorumlar");
        builder.Property(a => a.icerik).HasColumnType("text").Metadata.SetMaxLength(null);
    }
}
