using System.Collections.Generic;

namespace Services.SimpleTokenService
{
    public interface ISimpleTokenRepository
    {
        void Delete(ISimpleToken simpleToken);
        void DeleteAll(IEnumerable<ISimpleToken> simpleToken);
        ISimpleToken Get(int userId, string simpleToken);
        IEnumerable<ISimpleToken> GetByUserId(int userId);
        void Insert(int userId, string simpleToken);
        int Save();
    }
}