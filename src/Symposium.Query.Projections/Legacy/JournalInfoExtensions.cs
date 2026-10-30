using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class JournalInfoExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static JournalInfo ToJournalInfo(this journal_info a) => new JournalInfo
    {
        id = a.id,
        journal_id = a.journal_id,
        d_id = a.d_id,
        lang_id = a.lang_id,
        title = a.title,
        subtitle = a.subtitle,
        description = a.description,
        title_abbr = a.title_abbr,
        footer_text = a.footer_text,
        mail_signature = a.mail_signature,
        aim = a.aim,
        scope = a.scope,
        writing_rules = a.writing_rules,
        policy = a.policy,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
