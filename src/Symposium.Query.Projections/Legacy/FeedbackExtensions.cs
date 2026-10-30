using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class FeedbackExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Feedback ToFeedback(this feedbacks a) => new Feedback
    {
        id = a.id,
        grv_icerik_yonetici = a.grv_icerik_yonetici,
        grv_bitis_tarih = a.grv_bitis_tarih,
        grv_onemi = a.grv_onemi,
        slug = a.slug,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
