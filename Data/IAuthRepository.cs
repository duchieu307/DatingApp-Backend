using System.Threading.Tasks;
using Microservices.Models;

namespace Microservices.Data
{
    public interface IAuthRepository
    {
        // truyen vao 1 obj ten la user theo model User 
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}