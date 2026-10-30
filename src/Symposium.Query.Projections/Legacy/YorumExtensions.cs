using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class YorumExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Yorum ToYorum(this yorumlar a) => new Yorum
    {
        id = a.id,
        isim = a.isim,
        mail = a.mail,
        icerik = a.icerik,
        blog = a.blog,
        ust_yorum = a.ust_yorum,
        kullanici_id = a.kullanici_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
