using myAuthExampleApi.Models;
using System.Collections.Generic;

namespace myAuthExampleApi.Repositories
{
    public interface ISimpleTokenRepository
    {
        void Delete(int userId, string simpleToken);
        void DeleteAll(int userId);
        SimpleTokens Get(int userId, string simpleToken);
        IEnumerable<ISimpleTokens> GetAll(int userId);
        void Insert(int userId, string simpleToken);
        bool IsValid(int userId, string refreshToken);
        int Save();
    }
}