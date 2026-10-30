using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Symposium.Storage.Migrations
{
    /// <inheritdoc />
    public partial class CompleteSubmissionWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "faculty",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "gender",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "iletisim",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "institution",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "u_title",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "u_type",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "owner_id",
                table: "files",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "storage_file_id",
                table: "dosyalar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "authors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "submission_comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_submission_comments_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_submission_comments_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "submission_languages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    code = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    rtl = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "submission_notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    kind = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    recipient = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_attempt_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    claimed_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    attempts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_submission_notifications_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_submission_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_files_owner_id",
                table: "files",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_dosyalar_storage_file_id",
                table: "dosyalar",
                column: "storage_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_comments_article_id_created_at",
                table: "submission_comments",
                columns: new[] { "article_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_submission_comments_author_id",
                table: "submission_comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_languages_code",
                table: "submission_languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submission_notifications_article_id",
                table: "submission_notifications",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_notifications_sent_at_next_attempt_at",
                table: "submission_notifications",
                columns: new[] { "sent_at", "next_attempt_at" });

            migrationBuilder.CreateIndex(
                name: "IX_submission_notifications_user_id",
                table: "submission_notifications",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dosyalar_files_storage_file_id",
                table: "dosyalar",
                column: "storage_file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_files_users_owner_id",
                table: "files",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dosyalar_files_storage_file_id",
                table: "dosyalar");

            migrationBuilder.DropForeignKey(
                name: "FK_files_users_owner_id",
                table: "files");

            migrationBuilder.DropTable(
                name: "submission_comments");

            migrationBuilder.DropTable(
                name: "submission_languages");

            migrationBuilder.DropTable(
                name: "submission_notifications");

            migrationBuilder.DropIndex(
                name: "IX_files_owner_id",
                table: "files");

            migrationBuilder.DropIndex(
                name: "IX_dosyalar_storage_file_id",
                table: "dosyalar");

            migrationBuilder.DropColumn(
                name: "address",
                table: "users");

            migrationBuilder.DropColumn(
                name: "department",
                table: "users");

            migrationBuilder.DropColumn(
                name: "faculty",
                table: "users");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "users");

            migrationBuilder.DropColumn(
                name: "iletisim",
                table: "users");

            migrationBuilder.DropColumn(
                name: "institution",
                table: "users");

            migrationBuilder.DropColumn(
                name: "u_title",
                table: "users");

            migrationBuilder.DropColumn(
                name: "u_type",
                table: "users");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "storage_file_id",
                table: "dosyalar");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "authors");
        }
    }
}
