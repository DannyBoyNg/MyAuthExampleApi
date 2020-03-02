using myAuthExampleApi.Models.DbModels;
using Services.SimpleTokenServ;
using System.Collections.Generic;
using System.Linq;

namespace myAuthExampleApi.Repositories
{
    public class SimpleTokenRepository : ISimpleTokenRepository
    {
        private readonly MyAuthExampleContext db;

        public SimpleTokenRepository(MyAuthExampleContext db)
        {
            this.db = db;
        }

        public IEnumerable<ISimpleToken> GetByUserId(int userId)
        {
            return db.SimpleTokens.Where(x => x.UserId == userId).Cast<ISimpleToken>().ToList();
        }

        public void Insert(int userId, string simpleToken)
        {
            db.SimpleTokens.Add(new SimpleToken { UserId = userId, Token = simpleToken });
        }

        public void Delete(ISimpleToken simpleToken)
        {
            if (simpleToken != null) db.SimpleTokens.Remove(simpleToken as SimpleToken);
        }

        public void DeleteAll(IEnumerable<ISimpleToken> tokens)
        {
            db.SimpleTokens.RemoveRange(tokens.Cast<SimpleToken>());
        }

        //public bool IsValid(int userId, string refreshToken)
        //{
        //    var token = Get(userId, refreshToken);
        //    if (token != null)
        //    {
        //        Delete(token);
        //        return true;
        //    }
        //    return false;
        //}

        public int Save()
        {
            return db.SaveChanges();
        }
    }
}
