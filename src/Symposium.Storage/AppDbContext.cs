#pragma warning disable CS8981
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Symposium.Models;
using Symposium.Storage.Extensions;

namespace Symposium.Storage;

public class multi_string
{
    public string? en { get; set; }
}

public class content
{
    public bool is_html { get; set; }

    public multi_string body { get; set; }
}

public class vfile
{
    public int? owner_id { get; set; }
    [Key]
    public int id { get; set; }

    public string folder_id { get; set; }

    public string name { get; set; }

    public long size { get; set; }

    public string content_type { get; set; }

    public byte[]? hash { get; set; }

    public string access_url { get; set; }

    public string? download_url { get; set; }

    public DateTimeOffset created_at { get; set; }

    public DateTimeOffset? updated_at { get; set; }
}

public class user
{
    public string? address { get; set; }
    public string? institution { get; set; }
    public string? faculty { get; set; }
    public string? department { get; set; }
    public string? iletisim { get; set; }
    public int? u_title { get; set; }
    public int? u_type { get; set; }
    public int? gender { get; set; }
    [Key] public int id { get; set; }

    public string? first_name { get; set; }

    public string? last_name { get; set; }

    public string email { get; set; }

    public string password { get; set; }

    public DateTimeOffset? password_set_at { get; set; }

    public UserType type { get; set; }

    public DateTimeOffset created_at { get; set; }

    public DateTimeOffset? updated_at { get; set; }
}

public class token
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public Guid id { get; set; }

    public string password { get; set; }

    public bool shouldDispose { get; set; }

    public int user_id { get; set; }

    public Guid client_id { get; set; }

    public DateTimeOffset created_at { get; set; }

    public virtual user user { get; set; }
}

public class language
{
    [Key] public int id { get; set; }

    public bool enabled { get; set; }

    public int priority { get; set; }

    public string name { get; set; }

    public int? icon_id { get; set; }

    public virtual vfile? icon { get; set; }

    public string language_name { get; set; }

    public string language_code { get; set; }

    public int? parent_id { get; set; }

    public virtual language? parent { get; set; }

    public virtual List<language_value> values { get; set; }

    public DateTimeOffset created_at { get; set; }

    public DateTimeOffset? updated_at { get; set; }
}

public class language_key
{
    [Key] public string key { get; set; }

    public string? description { get; set; }

    public DateTimeOffset created_at { get; set; }

    public DateTimeOffset? updated_at { get; set; }
}

public class language_value
{
    public int language_id { get; set; }

    public virtual language language { get; set; }

    public string key { get; set; }

    public virtual language_key language_key { get; set; }

    public string value { get; set; }

    public DateTimeOffset created_at { get; set; }

    public DateTimeOffset? updated_at { get; set; }
}

public partial class AppDbContext : DbContext
{
    public virtual DbSet<token> tokens { get; set; }
    public virtual DbSet<user> users { get; set; }
    public virtual DbSet<vfile> files { get; set; }
    public virtual DbSet<language> languages { get; set; }
    public virtual DbSet<language_key> language_keys { get; set; }
    public virtual DbSet<language_value> language_values { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .HaveMaxLength(256);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<user>(user =>
        {
            user.Property<string>("normalized_email")
                .HasMaxLength(256)
                .HasComputedColumnSql("lower(btrim(email))", stored: true);
            user.HasIndex("normalized_email").IsUnique();
            user.ToTable(table => table.HasCheckConstraint("CK_users_email_not_blank", "btrim(email) <> ''"));

            user
                .Property(a => a.type)
                .HasConversion<int>();
        });

        modelBuilder.Entity<token>(token =>
        {
            token
                .HasOne(a => a.user)
                .WithMany()
                .HasForeignKey(a => a.user_id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<language>(language =>
        {
            language.ToTable("languages");

            language
                .HasOne(a => a.parent)
                .WithMany()
                .HasForeignKey(a => a.parent_id)
                .OnDelete(DeleteBehavior.Cascade);

            language
                .HasOne(a => a.icon)
                .WithMany()
                .HasForeignKey(a => a.icon_id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<language_key>(language_key =>
        {
            language_key.Property(a => a.description).HasColumnType("text").Metadata.SetMaxLength(null);
            language_key
                .HasIndex(a => a.key)
                .IsUnique();
        });

        modelBuilder.Entity<language_value>(language_value =>
        {
            language_value.Property(a => a.value).HasColumnType("text").Metadata.SetMaxLength(null);
            language_value
                .HasKey(a => new { a.key, a.language_id });

            language_value
                .HasOne(a => a.language)
                .WithMany(a => a.values)
                .HasForeignKey(a => a.language_id)
                .OnDelete(DeleteBehavior.Cascade);

            language_value
                .HasOne(a => a.language_key)
                .WithMany()
                .HasForeignKey(a => a.key)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<vfile>(vfile =>
        {
            vfile.Property(a => a.access_url).HasColumnType("text").Metadata.SetMaxLength(null);
            vfile.Property(a => a.download_url).HasColumnType("text").Metadata.SetMaxLength(null);
            vfile.ToTable(table => table.HasCheckConstraint("CK_files_size_nonnegative", "size >= 0"));
            vfile
                .Property(a => a.hash)
                .HasMaxLength(16)
                .IsFixedLength();
        });
    }
}
