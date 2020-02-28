using myAuthExampleApi.Models.DbModels;
using Services.UserService;
using System.Linq;

namespace myAuthExampleApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyAuthExampleContext db;

        public UserRepository(MyAuthExampleContext db)
        {
            this.db = db;
        }

        public void Delete(int userId)
        {
            var user = Get(userId);
            db.Users.Remove(user as User);
        }

        public IUser Get(int userId)
        {
            return db.Users.Where(x => x.Id == userId).SingleOrDefault();
        }

        public IUser GetByEmail(string email)
        {
            return db.Users.Where(x => x.Email == email).SingleOrDefault();
        }

        public IUser GetByName(string username)
        {
            return db.Users.Where(x => x.UserName == username).SingleOrDefault();
        }

        public void Insert(IUser user)
        {
            db.Users.Add(user as User);
            db.SaveChanges();
        }

        public bool IsEmailUnique(string email)
        {
            return !db.Users.Where(x => x.Email == email).Any();
        }

        public bool IsNameUnique(string username)
        {
            return !db.Users.Where(x => x.UserName == username).Any();
        }

        public int Save()
        {
            return db.SaveChanges();
        }
    }
}
