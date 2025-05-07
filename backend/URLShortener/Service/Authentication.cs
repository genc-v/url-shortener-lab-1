using System;
using System.Security.Claims;
using URLShortener.Service;

namespace URLShortener.Service
{
    public static class Authentication
    {
        public static int GetUserIdFromToken(string token, bool ignoreExpiration = false)
        {
            if (string.IsNullOrEmpty(token))
            {
                return -1;
            }

            var principal = TokenService.VerifyToken(token, ignoreExpiration);
            if (principal == null)
            {
                return -1;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return -1;
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return -1;
            }

            return userId;
        }

        public static bool IsAdminFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var principal = TokenService.VerifyToken(token);
            if (principal == null)
            {
                return false;
            }

            var isAdminClaim = principal.FindFirst("IsAdmin");
            if (isAdminClaim == null)
            {
                return false;
            }

            if (!bool.TryParse(isAdminClaim.Value, out bool isAdmin))
            {
                return false;
            }

            return isAdmin;
        }
    }
}

