using System.Security.Claims;
using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Controllers;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }
    
    public static UserRole GetUserRole(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.Role);
        if (claim != null && Enum.TryParse<UserRole>(claim.Value, out var role))
        {
            return role;
        }
        return UserRole.Customer;
    }
    
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }
}
