using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleToAuthorExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleToAuthor ToArticleToAuthor(this article_to_author a) => new ArticleToAuthor
    {
        id = a.id,
        article_id = a.article_id,
        author_id = a.author_id,
        order = a.order,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
