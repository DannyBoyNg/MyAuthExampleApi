using System.Collections.Generic;

namespace Services.JwtTokenServ
{
    public interface IRefreshTokenRepository
    {
        void Insert(int userId, string refreshToken);
        void Delete(IRefreshToken refreshToken);
        void DeleteAll(int userId);
        IEnumerable<IRefreshToken> GetByUserId(int userId);
        int Save();
    }
}