using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleToCitationExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleToCitation ToArticleToCitation(this article_to_citation a) => new ArticleToCitation
    {
        id = a.id,
        article_id = a.article_id,
        citation_id = a.citation_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
