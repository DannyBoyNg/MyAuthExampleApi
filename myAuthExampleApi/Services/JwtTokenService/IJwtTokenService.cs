using System.Collections.Generic;
using System.Security.Claims;

namespace Services.JwtTokenService
{
    public interface IJwtTokenService
    {
        string GetClaim(ClaimsPrincipal principal, string claimType);
        string GenerateAccessToken(string username, IEnumerable<Claim> claims = null);
        string GenerateRefreshToken();
        void StoreRefreshToken(int userId, string refreshToken);
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token);
    }
}
