using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class JournalsExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Journals ToJournals(this journals a) => new Journals
    {
        id = a.id,
        json_id = a.json_id,
        d_id = a.d_id,
        image_id = a.image_id,
        image_header_id = a.image_header_id,
        founded = a.founded,
        url = a.url,
        address = a.address,
        phone = a.phone,
        email = a.email,
        eissn = a.eissn,
        pissn = a.pissn,
        slug = a.slug,
        printed = a.printed,
        mandatory_lang = a.mandatory_lang,
        mandatory_rtl = a.mandatory_rtl,
        view_count = a.view_count,
        download_count = a.download_count,
        created_by = a.created_by,
        updated_by = a.updated_by,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
