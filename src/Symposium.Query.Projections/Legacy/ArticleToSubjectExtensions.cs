using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleToSubjectExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleToSubject ToArticleToSubject(this article_to_subject a) => new ArticleToSubject
    {
        id = a.id,
        article_id = a.article_id,
        subject_id = a.subject_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
