using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class KeywordsExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Keywords ToKeywords(this keywords a) => new Keywords
    {
        id = a.id,
        lang_id = a.lang_id,
        name = a.name,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
