using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleToArticleTypeExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleToArticleType ToArticleToArticleType(this article_to_article_type a) => new ArticleToArticleType
    {
        id = a.id,
        article_id = a.article_id,
        article_type_id = a.article_type_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
