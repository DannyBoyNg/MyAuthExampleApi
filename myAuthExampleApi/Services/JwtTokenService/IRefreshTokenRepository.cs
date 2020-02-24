using System.Collections.Generic;

namespace Services.JwtTokenService
{
    public interface IRefreshTokenRepository
    {
        void Insert(int userId, string refreshToken);
        void Delete(IRefreshToken refreshToken);
        void DeleteAll(int userId);
        IRefreshToken Get(int userId, string refreshToken);
        IEnumerable<IRefreshToken> GetByUserId(int userId);
        int Save();
    }
}