using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class JournalToArticleTypeExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static JournalToArticleType ToJournalToArticleType(this journal_to_article_type a) => new JournalToArticleType
    {
        id = a.id,
        journal_id = a.journal_id,
        article_type_id = a.article_type_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
