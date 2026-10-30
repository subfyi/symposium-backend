using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class TopicsExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Topics ToTopics(this topics a, AppDbContext db) => new Topics
    {
        lang = db.Set<submission_language>().Where(b => b.id.ToString() == a.type)
            .Select(b => new CatalogLanguage { id = b.id, name = b.name, code = b.code, active = b.active, rtl = b.rtl }).FirstOrDefault(),
        id = a.id,
        active = a.active,
        type = a.type,
        name = a.name,
        created_by = a.created_by,
        updated_by = a.updated_by,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
