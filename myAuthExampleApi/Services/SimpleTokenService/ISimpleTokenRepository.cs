using System.Collections.Generic;

namespace Services.SimpleTokenServ
{
    public interface ISimpleTokenRepository
    {
        void Delete(ISimpleToken simpleToken);
        void DeleteAll(IEnumerable<ISimpleToken> simpleToken);
        IEnumerable<ISimpleToken> GetByUserId(int userId);
        void Insert(int userId, string simpleToken);
        int Save();
    }
}