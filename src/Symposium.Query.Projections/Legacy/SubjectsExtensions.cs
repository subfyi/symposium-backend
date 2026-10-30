using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class SubjectsExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Subjects ToSubjects(this subjects a) => new Subjects
    {
        id = a.id,
        d_id = a.d_id,
        lang_id = a.lang_id,
        name = a.name,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
