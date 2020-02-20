using myAuthExampleApi.Models;
using System.Threading.Tasks;

namespace myAuthExampleApi.Repositories
{
    public interface IUserRepository
    {
        void Delete(int userId);
        IUsers Get(int userId);
        IUsers GetByEmail(string email);
        IUsers GetByName(string username);
        void Insert(IUsers user);
        bool IsEmailUnique(string email);
        bool IsNameUnique(string username);
        int Save();
        Task<int> SaveAsync();
    }
}