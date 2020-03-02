using System.Collections.Generic;
using System.Security.Claims;

namespace Services.JwtTokenServ
{
    public interface IJwtTokenService
    {
        string GetClaim(ClaimsPrincipal principal, string claimType);
        string GenerateAccessToken(string username, IEnumerable<Claim> claims = null);
        string GenerateRefreshToken();
        void StoreRefreshToken(int userId, string refreshToken);
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token);
        void ValidateRefreshToken(int userId, string refreshToken);
    }
}
