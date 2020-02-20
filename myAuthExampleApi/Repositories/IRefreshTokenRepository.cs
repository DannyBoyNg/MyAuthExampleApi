using myAuthExampleApi.Models;
using Services.JwtTokenService;
using System.Collections.Generic;

namespace myAuthExampleApi.Repositories
{
    public interface IRefreshTokenRepository
    {
        void Insert(int userId, string refreshToken);
        void Delete(int userId, string refreshToken);
        void DeleteAll(int userId);
        RefreshTokens Get(int userId, string refreshToken);
        IEnumerable<IRefreshTokens> GetAll(int userId);
        bool IsValid(int userId, string refreshToken);
        int Save();
    }
}