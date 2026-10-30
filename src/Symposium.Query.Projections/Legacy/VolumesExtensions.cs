using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class VolumesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Volumes ToVolumes(this volumes a) => new Volumes
    {
        id = a.id,
        journal_id = a.journal_id,
        d_id = a.d_id,
        volume = a.volume,
        issue = a.issue,
        special = a.special,
        year = a.year,
        long_year = a.long_year,
        short_year = a.short_year,
        json_id = a.json_id,
        created_by = a.created_by,
        date_published = a.date_published,
        season = a.season,
        supplement = a.supplement,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
