using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class CitationsExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Citations ToCitations(this citations a) => new Citations
    {
        id = a.id,
        article_id = a.article_id,
        d_id = a.d_id,
        raw = a.raw,
        type = a.type,
        order = a.order,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
