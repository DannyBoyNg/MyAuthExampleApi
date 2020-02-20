using myAuthExampleApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace myAuthExampleApi.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MyAuthExampleContext db;

        public RefreshTokenRepository(MyAuthExampleContext db)
        {
            this.db = db;
        }

        public RefreshTokens Get(int userId, string refreshToken)
        {
            return db.RefreshTokens.Where(x => x.UserId == userId && x.Token == refreshToken).SingleOrDefault();
        }

        public IEnumerable<IRefreshTokens> GetAll(int userId)
        {
            return db.RefreshTokens.Where(x => x.UserId == userId).Cast<IRefreshTokens>().ToList();
        }

        public void Insert(int userId, string refreshToken)
        {
            db.RefreshTokens.Add(new RefreshTokens { UserId = userId, Token = refreshToken });
        }

        public void Delete(int userId, string refreshToken)
        {
            var token = Get(userId, refreshToken);
            if (token != null) db.RefreshTokens.Remove(token);
        }

        public void DeleteAll(int userId) //only deleteAll refreshTokens if you want to log user off from all devices. mostly just for admins
        {
            var tokens = GetAll(userId).Cast<RefreshTokens>().ToList();
            if (tokens.Any()) db.RefreshTokens.RemoveRange(tokens);
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
