using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ParametersExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Parameters ToParameters(this parameters a) => new Parameters
    {
        id = a.id,
        type = a.type,
        value = a.value,
        item_num = a.item_num,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
