using System.Linq.Expressions;
using System.Security.Claims;
using LinqKit;

namespace Symposium.Api.Server.Extensions;

public static class QueryableExtensions
{
    // Example branch-scoped filtering; can be adapted for other multi-tenant patterns
    public static IQueryable<T> Where<T>(
        this IQueryable<T> that,
        ClaimsPrincipal user,
        Expression<Func<T, object>>? scopeFilter = null,
        string scopeClaimType = "scope_id")
    {
        // Symposium: add scope-based filtering as needed
        return that;
    }
}