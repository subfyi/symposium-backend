using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class JournalToPublisherExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static JournalToPublisher ToJournalToPublisher(this journal_to_publisher a) => new JournalToPublisher
    {
        id = a.id,
        journal_id = a.journal_id,
        publisher_id = a.publisher_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
