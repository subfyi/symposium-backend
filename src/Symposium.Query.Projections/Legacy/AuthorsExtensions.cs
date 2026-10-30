using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class AuthorsExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Authors ToAuthors(this authors a) => new Authors
    {
        id = a.id,
        p_id = a.p_id,
        p_user = a.p_user,
        first_name = a.first_name,
        last_name = a.last_name,
        adress = a.adress,
        email = a.email,
        correspond = a.correspond,
        presenter = a.presenter,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
