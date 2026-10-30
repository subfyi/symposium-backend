using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class GorevfilesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Gorevfiles ToGorevfiles(this gorev_files a) => new Gorevfiles
    {
        id = a.id,
        f_icerik_id = a.f_icerik_id,
        f_icerik_olusturan = a.f_icerik_olusturan,
        f_dosyayeri = a.f_dosyayeri,
        r_dosyaisimi = a.r_dosyaisimi,
        f_dosyaisimi = a.f_dosyaisimi,
        f_malzeme_turu = a.f_malzeme_turu,
        f_baski_olcusu = a.f_baski_olcusu,
        f_urun_olcusu = a.f_urun_olcusu,
        f_uygulama = a.f_uygulama,
        f_adet = a.f_adet,
        slug = a.slug,
        f_dosya_uzanti = a.f_dosya_uzanti,
        f_dosya_boyut = a.f_dosya_boyut,
        f_urun_olcusu_b = a.f_urun_olcusu_b,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
