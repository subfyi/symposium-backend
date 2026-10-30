using EntityFrameworkCore.Projectables;
using Symposium.Storage;
using File = Symposium.Models.File;

namespace Symposium.Query.Projections;

public static class FileExtensions
{
    [Projectable]
    public static File ToFile(this vfile? a) => a == null ? null : new File
    {
        id = a.id,
        name = a.name,
        size = a.size,
        access_url = a.access_url,
        download_url = a.download_url
    };

    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static string ToUrl(this vfile a)
        => a?.access_url;
}