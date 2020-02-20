using myAuthExampleApi.Models;
using System.Linq;
using System.Threading.Tasks;

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
            db.Users.Remove(user as Users);
        }

        public IUsers Get(int userId)
        {
            return db.Users.Where(x => x.Id == userId).SingleOrDefault();
        }

        public IUsers GetByEmail(string email)
        {
            return db.Users.Where(x => x.Email == email).SingleOrDefault();
        }

        public IUsers GetByName(string username)
        {
            return db.Users.Where(x => x.UserName == username).SingleOrDefault();
        }

        public void Insert(IUsers user)
        {
            db.Users.Add(user as Users);
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

        public async Task<int> SaveAsync()
        {
            return await db.SaveChangesAsync();
        }
    }
}
