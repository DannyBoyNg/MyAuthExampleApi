namespace Services.UserService
{
    public interface IUserRepository
    {
        void Delete(int userId);
        IUser Get(int userId);
        IUser GetByEmail(string email);
        IUser GetByName(string username);
        void Insert(IUser user);
        bool IsEmailUnique(string email);
        bool IsNameUnique(string username);
        int Save();
    }
}