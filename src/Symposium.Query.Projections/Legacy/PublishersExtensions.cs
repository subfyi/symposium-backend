using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class PublishersExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Publishers ToPublishers(this publishers a) => new Publishers
    {
        id = a.id,
        d_id = a.d_id,
        lang_id = a.lang_id,
        name = a.name,
        email = a.email,
        lft = a.lft,
        address_lat = a.address_lat,
        address_long = a.address_long,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
