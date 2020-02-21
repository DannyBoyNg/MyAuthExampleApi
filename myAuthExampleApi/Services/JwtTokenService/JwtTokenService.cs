using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using myAuthExampleApi.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Services.JwtTokenService
{
    public class JwtTokenService : IJwtTokenService
    {
        public JwtTokenSettings Settings { get; set; }
        public IRefreshTokenRepository RefreshTokenRepo { get; }

        public JwtTokenService(
            IOptions<JwtTokenSettings> settings,
            IRefreshTokenRepository refreshTokenRepo)
        {
            Settings = settings.Value;
            RefreshTokenRepo = refreshTokenRepo;
        }

        public int GetUserId(ClaimsPrincipal user)
        {
            if (!int.TryParse(GetClaim(user, "uid"), out int uid)) throw new Exception();
            return uid;
        }

        public string GetClaim(ClaimsPrincipal claimsIdentity, string claimType)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }

        public string GenerateAccessToken(string username, IEnumerable<Claim> extraClaims = null)
        {
            var issuedAt = DateTime.UtcNow;
            var issuedAtUnix = ((DateTimeOffset)issuedAt).ToUnixTimeSeconds();
            var expiresAt = issuedAt.AddMinutes(Settings.AccessTokenExpirationInMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, issuedAtUnix.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, username),
            };
            if (extraClaims != null && extraClaims.Any()) claims.AddRange(extraClaims.ToList());

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Key));
            var token = new JwtSecurityToken(
              issuer: Settings.Issuer,
              audience: Settings.Audience,
              claims: claims,
              notBefore: issuedAt,
              expires: expiresAt,
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Key)),
                ValidateLifetime = false,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid access token");
            }
            return principal;
        }

        public string GenerateRefreshToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray()).Replace('/', '_').Replace('+', '-');
        }

        public void StoreRefreshToken(int userId, string refreshToken)
        {
            RefreshTokenRepo.Insert(userId, refreshToken);
            RefreshTokenRepo.Save();
        }

        public DateTime GetCreationTimeFromRefreshToken(string refreshToken)
        {
            if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));
            refreshToken = refreshToken.Replace('_', '/').Replace('-', '+');
            switch (refreshToken.Length % 4)
            {
                case 2: refreshToken += "=="; break;
                case 3: refreshToken += "="; break;
            }
            byte[] data = Convert.FromBase64String(refreshToken);
            return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        }

        public bool IsRefreshTokenExpired(string refreshToken)
        {
            DateTime when = GetCreationTimeFromRefreshToken(refreshToken);
            return when < DateTime.UtcNow.AddHours(Settings.RefreshTokenExpirationInHours * -1);
        }

        public bool IsRefreshTokenValid(int userId, string refreshToken)
        {
            var token = RefreshTokenRepo.Get(userId, refreshToken);
            if (token != null) RefreshTokenRepo.Delete(token);
            if (token == null || IsRefreshTokenExpired(refreshToken)) return false;
            RefreshTokenRepo.Save();
            return true;
        }
    }
}
