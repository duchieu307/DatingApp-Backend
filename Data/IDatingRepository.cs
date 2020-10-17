using System.Collections.Generic;
using System.Threading.Tasks;
using Microservices.Helpers;
using Microservices.Models;

namespace Microservices.Data
{
    public interface IDatingRepository
    {
        // generic method tao ra mot method hoan lai viec khai bao cac kieu du lieu 
        // den khi ham dc thuc thi
         void Add<T>(T entity) where T:class;
         void Delete<T>(T entity) where T:class;
         Task<bool> SaveAll();
         Task<PagedList<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);

        Task<Photo> GetPhoto(int id);

        Task<Photo> GetMainPhotoForUser(int userId);

        Task<Like> GetLike(int userId, int recipientId);

        Task<Message> GetMessage(int id);

        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);

        Task<IEnumerable<Message>> GetMessageThread(int useriD, int recipientId);
    }
}