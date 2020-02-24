namespace Services.UserService
{
    public interface IUserService
    {
        IUser GetById(int userId);
        IUser GetByName(string username);
        IUser GetByEmail(string email);
        bool IsNameUnique(string username);
        bool IsEmailUnique(string email);
        bool IsEmailConfirmed(int userId);
        void SetEmailConfirmed(int userId);
        void UpdatePassword(int userId, string PasswordHash);
        void Create(IUser user);
    }
}