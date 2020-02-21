using myAuthExampleApi.Models;
using System.Collections.Generic;

namespace myAuthExampleApi.Repositories
{
    public interface ISimpleTokenRepository
    {
        void Delete(ISimpleTokens simpleToken);
        void DeleteAll(IEnumerable<ISimpleTokens> simpleToken);
        SimpleTokens Get(int userId, string simpleToken);
        IEnumerable<ISimpleTokens> GetByUserId(int userId);
        void Insert(int userId, string simpleToken);
        //bool IsValid(int userId, string refreshToken);
        int Save();
    }
}