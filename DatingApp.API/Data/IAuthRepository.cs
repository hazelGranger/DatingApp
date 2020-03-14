using System.Threading.Tasks;
using DatingApp.API.Model.Model;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string user, string password);
        Task<bool> UserExits(string username);
    }
}