using myAuthExampleApi.Models.DbModels;
using DannyBoyNg.Services;
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

        public IEnumerable<IRefreshToken> GetByUserId(int userId)
        {
            return db.RefreshTokens.Where(x => x.UserId == userId).Cast<IRefreshToken>().ToList();
        }

        public void Insert(int userId, string refreshToken)
        {
            db.RefreshTokens.Add(new RefreshToken { UserId = userId, Token = refreshToken });
        }

        public void Delete(IRefreshToken token)
        {
            if (token != null) db.RefreshTokens.Remove(token as RefreshToken);
        }

        public void DeleteAll(int userId) //only deleteAll refreshTokens if you want to log user off from all devices. mostly just for admins
        {
            var tokens = GetByUserId(userId).Cast<RefreshToken>().ToList();
            if (tokens.Any()) db.RefreshTokens.RemoveRange(tokens);
        }

        public int Save()
        {
            return db.SaveChanges();
        }
    }
}
