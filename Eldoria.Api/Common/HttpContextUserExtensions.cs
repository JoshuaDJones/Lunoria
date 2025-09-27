using System.Security.Claims;

namespace Eldoria.Api.Common
{
    public static class HttpContextUserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub")
                ?? user.FindFirstValue("uid");

            return int.Parse(value!);
        }
    }
}
