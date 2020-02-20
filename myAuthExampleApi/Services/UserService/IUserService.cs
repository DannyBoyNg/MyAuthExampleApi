using myAuthExampleApi.Models;

namespace Services.UserService
{
    public interface IUserService
    {
        IUsers GetById(int userId);
        IUsers GetByName(string username);
        IUsers GetByEmail(string email);
        bool IsNameUnique(string username);
        bool IsEmailUnique(string email);
        bool IsEmailConfirmed(int userId);
        void SetEmailConfirmed(int userId);
        void UpdatePassword(int userId, string PasswordHash);
        void Create(IUsers user);
    }
}