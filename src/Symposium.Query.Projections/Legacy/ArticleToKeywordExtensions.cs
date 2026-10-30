using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleToKeywordExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleToKeyword ToArticleToKeyword(this article_to_keyword a) => new ArticleToKeyword
    {
        id = a.id,
        article_id = a.article_id,
        keyword_id = a.keyword_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
