using myAuthExampleApi.Models;
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

        public SimpleTokens Get(int userId, string simpleToken)
        {
            return db.SimpleTokens.Where(x => x.UserId == userId && x.Token == simpleToken).SingleOrDefault();
        }

        public IEnumerable<ISimpleTokens> GetAll(int userId)
        {
            return db.SimpleTokens.Where(x => x.UserId == userId).Cast<ISimpleTokens>().ToList();
        }

        public void Insert(int userId, string simpleToken)
        {
            db.SimpleTokens.Add(new SimpleTokens { UserId = userId, Token = simpleToken });
        }

        public void Delete(int userId, string simpleToken)
        {
            var token = Get(userId, simpleToken);
            if (token != null) db.SimpleTokens.Remove(token);
        }

        public void DeleteAll(int userId)
        {
            var tokens = GetAll(userId).Cast<SimpleTokens>().ToList();
            if (tokens.Any()) db.SimpleTokens.RemoveRange(tokens);
        }

        public bool IsValid(int userId, string refreshToken)
        {
            var token = Get(userId, refreshToken);
            if (token != null)
            {
                Delete(userId, refreshToken);
                return true;
            }
            return false;
        }

        public int Save()
        {
            return db.SaveChanges();
        }
    }
}
