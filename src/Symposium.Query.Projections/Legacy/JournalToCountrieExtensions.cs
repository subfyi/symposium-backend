using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class JournalToCountrieExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static JournalToCountrie ToJournalToCountrie(this journal_to_countrie a) => new JournalToCountrie
    {
        id = a.id,
        journal_id = a.journal_id,
        countrie_id = a.countrie_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
