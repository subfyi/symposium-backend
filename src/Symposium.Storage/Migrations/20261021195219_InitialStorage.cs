using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Symposium.Storage.Migrations
{
    /// <inheritdoc />
    public partial class InitialStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "article_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    provinces = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dosyalar",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    f_icerik_olusturan = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosyayeri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    r_dosyaisimi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosyaisimi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    g_dosyaismi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_klasorismi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    g_klasorismi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosya_uzanti = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosya_boyut = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    width = table.Column<int>(type: "integer", nullable: true),
                    height = table.Column<int>(type: "integer", nullable: true),
                    f_sira = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dosyalar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feedbackdata",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    isicerik = table.Column<string>(type: "text", nullable: true),
                    slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ust_icerik = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grv_icerik_olusturan = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    dosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    gdosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    r_dosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    r_gdosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbackdata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    grv_icerik_yonetici = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grv_bitis_tarih = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grv_onemi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folder_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    content_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    hash = table.Column<byte[]>(type: "bytea", fixedLength: true, maxLength: 16, nullable: true),
                    access_url = table.Column<string>(type: "text", nullable: false),
                    download_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.id);
                    table.CheckConstraint("CK_files_size_nonnegative", "size >= 0");
                });

            migrationBuilder.CreateTable(
                name: "gorev_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    f_icerik_id = table.Column<int>(type: "integer", nullable: true),
                    f_icerik_olusturan = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosyayeri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    r_dosyaisimi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosyaisimi = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_malzeme_turu = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_baski_olcusu = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_urun_olcusu = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_uygulama = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_adet = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosya_uzanti = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_dosya_boyut = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    f_urun_olcusu_b = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gorev_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gorev_icerik",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    isicerik = table.Column<string>(type: "text", nullable: true),
                    slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ust_icerik = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grv_icerik_olusturan = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    dosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    gdosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    r_dosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    r_gdosyaisimleri = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gorev_icerik", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    json_id = table.Column<int>(type: "integer", nullable: true),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    image_id = table.Column<int>(type: "integer", nullable: true),
                    image_header_id = table.Column<int>(type: "integer", nullable: true),
                    founded = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    eissn = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pissn = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    printed = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    mandatory_lang = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    mandatory_rtl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    view_count = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    download_count = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "keywords",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_keywords", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "language_keys",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language_keys", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "parameters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    item_num = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parameters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "presentations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    active = table.Column<bool>(type: "boolean", nullable: true),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presentations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "publishers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    lft = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    address_lat = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    address_long = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publishers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "publishs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    active = table.Column<bool>(type: "boolean", nullable: true),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publishs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    active = table.Column<bool>(type: "boolean", nullable: true),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    value_tr = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    value_en = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    last_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_set_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true, computedColumnSql: "lower(btrim(email))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.CheckConstraint("CK_users_email_not_blank", "btrim(email) <> ''");
                });

            migrationBuilder.CreateTable(
                name: "volume_to_year",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    volume_num = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    year = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    short_year = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volume_to_year", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "yorumlar",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    isim = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    mail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    icerik = table.Column<string>(type: "text", nullable: true),
                    blog = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ust_yorum = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    kullanici_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yorumlar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    en_title = table.Column<string>(type: "text", nullable: true),
                    pap_topic = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pap_creater = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pap_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pre_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pap_status = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pap_payment = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    en_abstract = table.Column<string>(type: "text", nullable: true),
                    pap_keyword = table.Column<string>(type: "text", nullable: true),
                    pap_topic_p = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    mes_congress = table.Column<string>(type: "text", nullable: true),
                    abstract_dosya_id = table.Column<int>(type: "integer", nullable: true),
                    full_paper_dosya_id = table.Column<int>(type: "integer", nullable: true),
                    poster_presentation_id = table.Column<int>(type: "integer", nullable: true),
                    video_id = table.Column<int>(type: "integer", nullable: true),
                    video_approved = table.Column<bool>(type: "boolean", nullable: true),
                    title_year = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    paper_approved = table.Column<bool>(type: "boolean", nullable: true),
                    paper_page = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    paper_page_end = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    paper_abst_page = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    paper_external = table.Column<string>(type: "text", nullable: true),
                    pap_num = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.id);
                    table.ForeignKey(
                        name: "FK_articles_dosyalar_abstract_dosya_id",
                        column: x => x.abstract_dosya_id,
                        principalTable: "dosyalar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_articles_dosyalar_full_paper_dosya_id",
                        column: x => x.full_paper_dosya_id,
                        principalTable: "dosyalar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_articles_dosyalar_poster_presentation_id",
                        column: x => x.poster_presentation_id,
                        principalTable: "dosyalar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_articles_dosyalar_video_id",
                        column: x => x.video_id,
                        principalTable: "dosyalar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    icon_id = table.Column<int>(type: "integer", nullable: true),
                    language_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    language_code = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.id);
                    table.ForeignKey(
                        name: "FK_languages_files_icon_id",
                        column: x => x.icon_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_languages_languages_parent_id",
                        column: x => x.parent_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journal_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_id = table.Column<int>(type: "integer", nullable: true),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    subtitle = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    title_abbr = table.Column<string>(type: "text", nullable: true),
                    footer_text = table.Column<string>(type: "text", nullable: true),
                    mail_signature = table.Column<string>(type: "text", nullable: true),
                    aim = table.Column<string>(type: "text", nullable: true),
                    scope = table.Column<string>(type: "text", nullable: true),
                    writing_rules = table.Column<string>(type: "text", nullable: true),
                    policy = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_info_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_to_article_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_id = table.Column<int>(type: "integer", nullable: true),
                    article_type_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_to_article_type", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_to_article_type_article_types_article_type_id",
                        column: x => x.article_type_id,
                        principalTable: "article_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journal_to_article_type_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_to_countrie",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_id = table.Column<int>(type: "integer", nullable: true),
                    countrie_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_to_countrie", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_to_countrie_countries_countrie_id",
                        column: x => x.countrie_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journal_to_countrie_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "volumes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_id = table.Column<int>(type: "integer", nullable: true),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    volume = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    issue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    special = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    year = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    long_year = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    short_year = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    json_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    date_published = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    season = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    supplement = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volumes", x => x.id);
                    table.ForeignKey(
                        name: "FK_volumes_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_to_publisher",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_id = table.Column<int>(type: "integer", nullable: true),
                    publisher_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_to_publisher", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_to_publisher_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journal_to_publisher_publishers_publisher_id",
                        column: x => x.publisher_id,
                        principalTable: "publishers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_to_subject",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_id = table.Column<int>(type: "integer", nullable: true),
                    subject_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_to_subject", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_to_subject_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journal_to_subject_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    shouldDispose = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "article_to_article_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    article_type_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_to_article_type", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_to_article_type_article_types_article_type_id",
                        column: x => x.article_type_id,
                        principalTable: "article_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_to_article_type_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "article_to_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    file_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_to_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_to_files_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_to_files_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "article_to_keyword",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    keyword_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_to_keyword", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_to_keyword_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_to_keyword_keywords_keyword_id",
                        column: x => x.keyword_id,
                        principalTable: "keywords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "article_to_subject",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    subject_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_to_subject", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_to_subject_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_to_subject_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    p_id = table.Column<int>(type: "integer", nullable: true),
                    p_user = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    first_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    last_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    adress = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    correspond = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    presenter = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.id);
                    table.ForeignKey(
                        name: "FK_authors_articles_p_id",
                        column: x => x.p_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "citations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    raw = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    order = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citations", x => x.id);
                    table.ForeignKey(
                        name: "FK_citations_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "language_values",
                columns: table => new
                {
                    language_id = table.Column<int>(type: "integer", nullable: false),
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language_values", x => new { x.key, x.language_id });
                    table.ForeignKey(
                        name: "FK_language_values_language_keys_key",
                        column: x => x.key,
                        principalTable: "language_keys",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_language_values_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "article_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    v_id = table.Column<int>(type: "integer", nullable: true),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    @abstract = table.Column<string>(name: "abstract", type: "text", nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_info_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_info_volumes_v_id",
                        column: x => x.v_id,
                        principalTable: "volumes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "volume_to_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    volume_id = table.Column<int>(type: "integer", nullable: true),
                    d_id = table.Column<int>(type: "integer", nullable: true),
                    lang_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    file_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volume_to_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_volume_to_files_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_volume_to_files_volumes_volume_id",
                        column: x => x.volume_id,
                        principalTable: "volumes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "article_to_author",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    author_id = table.Column<int>(type: "integer", nullable: true),
                    order = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_to_author", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_to_author_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_to_author_authors_author_id",
                        column: x => x.author_id,
                        principalTable: "authors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "article_to_citation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: true),
                    citation_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_to_citation", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_to_citation_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_to_citation_citations_citation_id",
                        column: x => x.citation_id,
                        principalTable: "citations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_article_info_article_id",
                table: "article_info",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_info_v_id",
                table: "article_info",
                column: "v_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_article_type_article_id",
                table: "article_to_article_type",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_article_type_article_type_id",
                table: "article_to_article_type",
                column: "article_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_author_article_id",
                table: "article_to_author",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_author_author_id",
                table: "article_to_author",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_citation_article_id",
                table: "article_to_citation",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_citation_citation_id",
                table: "article_to_citation",
                column: "citation_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_files_article_id",
                table: "article_to_files",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_files_file_id",
                table: "article_to_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_keyword_article_id",
                table: "article_to_keyword",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_keyword_keyword_id",
                table: "article_to_keyword",
                column: "keyword_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_subject_article_id",
                table: "article_to_subject",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_to_subject_subject_id",
                table: "article_to_subject",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_articles_abstract_dosya_id",
                table: "articles",
                column: "abstract_dosya_id");

            migrationBuilder.CreateIndex(
                name: "IX_articles_full_paper_dosya_id",
                table: "articles",
                column: "full_paper_dosya_id");

            migrationBuilder.CreateIndex(
                name: "IX_articles_poster_presentation_id",
                table: "articles",
                column: "poster_presentation_id");

            migrationBuilder.CreateIndex(
                name: "IX_articles_video_id",
                table: "articles",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "IX_authors_p_id",
                table: "authors",
                column: "p_id");

            migrationBuilder.CreateIndex(
                name: "IX_citations_article_id",
                table: "citations",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_info_journal_id",
                table: "journal_info",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_article_type_article_type_id",
                table: "journal_to_article_type",
                column: "article_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_article_type_journal_id",
                table: "journal_to_article_type",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_countrie_countrie_id",
                table: "journal_to_countrie",
                column: "countrie_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_countrie_journal_id",
                table: "journal_to_countrie",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_publisher_journal_id",
                table: "journal_to_publisher",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_publisher_publisher_id",
                table: "journal_to_publisher",
                column: "publisher_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_subject_journal_id",
                table: "journal_to_subject",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_to_subject_subject_id",
                table: "journal_to_subject",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_language_keys_key",
                table: "language_keys",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_language_values_language_id",
                table: "language_values",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "IX_languages_icon_id",
                table: "languages",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "IX_languages_parent_id",
                table: "languages",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_user_id",
                table: "tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_email",
                table: "users",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_volume_to_files_file_id",
                table: "volume_to_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_volume_to_files_volume_id",
                table: "volume_to_files",
                column: "volume_id");

            migrationBuilder.CreateIndex(
                name: "IX_volumes_journal_id",
                table: "volumes",
                column: "journal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_info");

            migrationBuilder.DropTable(
                name: "article_to_article_type");

            migrationBuilder.DropTable(
                name: "article_to_author");

            migrationBuilder.DropTable(
                name: "article_to_citation");

            migrationBuilder.DropTable(
                name: "article_to_files");

            migrationBuilder.DropTable(
                name: "article_to_keyword");

            migrationBuilder.DropTable(
                name: "article_to_subject");

            migrationBuilder.DropTable(
                name: "feedbackdata");

            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "gorev_files");

            migrationBuilder.DropTable(
                name: "gorev_icerik");

            migrationBuilder.DropTable(
                name: "journal_info");

            migrationBuilder.DropTable(
                name: "journal_to_article_type");

            migrationBuilder.DropTable(
                name: "journal_to_countrie");

            migrationBuilder.DropTable(
                name: "journal_to_publisher");

            migrationBuilder.DropTable(
                name: "journal_to_subject");

            migrationBuilder.DropTable(
                name: "language_values");

            migrationBuilder.DropTable(
                name: "parameters");

            migrationBuilder.DropTable(
                name: "presentations");

            migrationBuilder.DropTable(
                name: "publishs");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "topics");

            migrationBuilder.DropTable(
                name: "types");

            migrationBuilder.DropTable(
                name: "volume_to_files");

            migrationBuilder.DropTable(
                name: "volume_to_year");

            migrationBuilder.DropTable(
                name: "yorumlar");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "citations");

            migrationBuilder.DropTable(
                name: "keywords");

            migrationBuilder.DropTable(
                name: "article_types");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "publishers");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "language_keys");

            migrationBuilder.DropTable(
                name: "languages");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "volumes");

            migrationBuilder.DropTable(
                name: "articles");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "journals");

            migrationBuilder.DropTable(
                name: "dosyalar");
        }
    }
}
