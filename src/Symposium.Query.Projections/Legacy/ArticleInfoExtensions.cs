using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleInfoExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleInfo ToArticleInfo(this article_info a) => new ArticleInfo
    {
        id = a.id,
        d_id = a.d_id,
        v_id = a.v_id,
        article_id = a.article_id,
        lang_id = a.lang_id,
        title = a.title,
        @abstract = a.@abstract,
        created_by = a.created_by,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
