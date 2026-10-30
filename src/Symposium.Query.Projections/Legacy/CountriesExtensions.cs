using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class CountriesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Countries ToCountries(this countries a) => new Countries
    {
        id = a.id,
        d_id = a.d_id,
        name = a.name,
        provinces = a.provinces,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
