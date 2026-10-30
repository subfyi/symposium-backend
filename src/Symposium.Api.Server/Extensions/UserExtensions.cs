using System.Security.Claims;

namespace Symposium.Api.Server.Extensions;

public static class UserExtensions
{
    public static int? GetBranchId(this ClaimsPrincipal user, int? defaultValue = null)
    {
        var branchId = user.FindFirst("branch_id")?.Value is {} _branchId ? int.Parse(_branchId) : default(int?);
        return branchId ?? defaultValue;
    }
}