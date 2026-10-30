using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class VolumeToYearExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static VolumeToYear ToVolumeToYear(this volume_to_year a) => new VolumeToYear
    {
        id = a.id,
        volume_num = a.volume_num,
        year = a.year,
        short_year = a.short_year,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
