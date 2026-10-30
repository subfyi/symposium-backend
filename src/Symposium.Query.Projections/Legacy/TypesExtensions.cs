using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class TypesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Types ToTypes(this types a) => new Types
    {
        id = a.id,
        d_id = a.d_id,
        value_tr = a.value_tr,
        value_en = a.value_en,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
