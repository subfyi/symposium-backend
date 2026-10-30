using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class DosyalarExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Dosyalar ToDosyalar(this dosyalar a) => new Dosyalar
    {
        id = a.id,
        f_icerik_olusturan = a.f_icerik_olusturan,
        f_dosyayeri = a.f_dosyayeri,
        r_dosyaisimi = a.r_dosyaisimi,
        f_dosyaisimi = a.f_dosyaisimi,
        g_dosyaismi = a.g_dosyaismi,
        f_klasorismi = a.f_klasorismi,
        g_klasorismi = a.g_klasorismi,
        f_dosya_uzanti = a.f_dosya_uzanti,
        f_dosya_boyut = a.f_dosya_boyut,
        width = a.width,
        height = a.height,
        f_sira = a.f_sira,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
