using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Projectables.Infrastructure;
using Symposium.Storage;
using Symposium.Query.Projections;

await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql("Host=127.0.0.1;Port=1;Database=unused;Username=unused")
    .UseProjectables(a => a.CompatibilityMode(CompatibilityMode.Full)).Options);
var publicSql = db.Set<articles>().PublishedArticles("2023", orals: true).OrderBy(a => a.id).Take(15).ToPublicArticles(db).ToQueryString();
if (publicSql.Contains("email") || publicSql.Contains("pap_creater") || publicSql.Contains("submission_comments"))
    throw new Exception("Private data is selected by the public projection.");
var privateSql = db.Set<articles>().OrderBy(a => a.id).Take(15).ToAdminArticles(db).ToQueryString();
if (!privateSql.Contains("email") || !privateSql.Contains("submission_comments") || !privateSql.Contains("sort_order"))
    throw new Exception("Private submission projection is incomplete.");
Console.WriteLine("Article SQL projection compatibility checks passed using the real storage model; no database connection was opened.");
