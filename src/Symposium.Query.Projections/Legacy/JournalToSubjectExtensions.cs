using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class JournalToSubjectExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static JournalToSubject ToJournalToSubject(this journal_to_subject a) => new JournalToSubject
    {
        id = a.id,
        journal_id = a.journal_id,
        subject_id = a.subject_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
