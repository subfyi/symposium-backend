using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class VolumeToFilesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static VolumeToFiles ToVolumeToFiles(this volume_to_files a) => new VolumeToFiles
    {
        id = a.id,
        volume_id = a.volume_id,
        d_id = a.d_id,
        lang_id = a.lang_id,
        title = a.title,
        description = a.description,
        file_id = a.file_id,
        type = a.type,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
