using Microsoft.EntityFrameworkCore;

namespace Symposium.Storage;

public partial class AppDbContext
{
    public virtual DbSet<article_info> article_info { get; set; }
    public virtual DbSet<article_to_article_type> article_to_article_type { get; set; }
    public virtual DbSet<article_to_author> article_to_author { get; set; }
    public virtual DbSet<article_to_citation> article_to_citation { get; set; }
    public virtual DbSet<article_to_files> article_to_files { get; set; }
    public virtual DbSet<article_to_keyword> article_to_keyword { get; set; }
    public virtual DbSet<article_to_subject> article_to_subject { get; set; }
    public virtual DbSet<article_types> article_types { get; set; }
    public virtual DbSet<articles> articles { get; set; }
    public virtual DbSet<authors> authors { get; set; }
    public virtual DbSet<citations> citations { get; set; }
    public virtual DbSet<countries> countries { get; set; }
    public virtual DbSet<dosyalar> dosyalar { get; set; }
    public virtual DbSet<feedbacks> feedbacks { get; set; }
    public virtual DbSet<feedbackdata> feedbackdata { get; set; }
    public virtual DbSet<gorev_files> gorev_files { get; set; }
    public virtual DbSet<gorev_icerik> gorev_icerik { get; set; }
    public virtual DbSet<journal_info> journal_info { get; set; }
    public virtual DbSet<journal_to_article_type> journal_to_article_type { get; set; }
    public virtual DbSet<journal_to_countrie> journal_to_countrie { get; set; }
    public virtual DbSet<journal_to_publisher> journal_to_publisher { get; set; }
    public virtual DbSet<journal_to_subject> journal_to_subject { get; set; }
    public virtual DbSet<journals> journals { get; set; }
    public virtual DbSet<keywords> keywords { get; set; }
    public virtual DbSet<parameters> parameters { get; set; }
    public virtual DbSet<presentations> presentations { get; set; }
    public virtual DbSet<publishers> publishers { get; set; }
    public virtual DbSet<publishs> publishs { get; set; }
    public virtual DbSet<roles> roles { get; set; }
    public virtual DbSet<subjects> subjects { get; set; }
    public virtual DbSet<topics> topics { get; set; }
    public virtual DbSet<types> types { get; set; }
    public virtual DbSet<volume_to_files> volume_to_files { get; set; }
    public virtual DbSet<volume_to_year> volume_to_year { get; set; }
    public virtual DbSet<volumes> volumes { get; set; }
    public virtual DbSet<yorumlar> yorumlar { get; set; }
}
