using myAuthExampleApi.Models;
using myAuthExampleApi.Repositories;

namespace Services.UserService
{
    public class UserService : IUserService
    {
        public IUserRepository UserRepo { get; }

        public UserService(IUserRepository userRepo)
        {
            UserRepo = userRepo;
        }

        public void Create(IUsers user)
        {
            UserRepo.Insert(user as Users);
        }

        public IUsers GetByEmail(string email)
        {
            return UserRepo.GetByEmail(email);
        }

        public IUsers GetById(int userId)
        {
            return UserRepo.Get(userId);
        }

        public IUsers GetByName(string username)
        {
            return UserRepo.GetByName(username);
        }

        public bool IsEmailUnique(string email)
        {
            return UserRepo.IsEmailUnique(email);
        }

        public bool IsNameUnique(string username)
        {
            return UserRepo.IsNameUnique(username);
        }

        public void UpdatePassword(int userId, string PasswordHash)
        {
            var user = GetById(userId);
            user.PasswordHash = PasswordHash;
            UserRepo.SaveAsync();
        }

        public bool IsEmailConfirmed(int userId)
        {
            var user = GetById(userId);
            return user.EmailConfirmed;
        }

        public void SetEmailConfirmed(int userId)
        {
            var user = GetById(userId);
            user.EmailConfirmed = true;
            UserRepo.SaveAsync();
        }
    }
}
