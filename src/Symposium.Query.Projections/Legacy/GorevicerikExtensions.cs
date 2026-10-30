using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class GorevicerikExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Gorevicerik ToGorevicerik(this gorev_icerik a) => new Gorevicerik
    {
        id = a.id,
        isicerik = a.isicerik,
        slug = a.slug,
        ust_icerik = a.ust_icerik,
        grv_icerik_olusturan = a.grv_icerik_olusturan,
        dosyaisimleri = a.dosyaisimleri,
        gdosyaisimleri = a.gdosyaisimleri,
        r_dosyaisimleri = a.r_dosyaisimleri,
        r_gdosyaisimleri = a.r_gdosyaisimleri,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
