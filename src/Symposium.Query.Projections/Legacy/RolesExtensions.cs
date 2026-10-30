using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class RolesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Roles ToRoles(this roles a) => new Roles
    {
        id = a.id,
        name = a.name,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
