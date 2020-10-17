using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservices.Helpers;
using Microservices.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Data
{
    // nho them repository vao service
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(user => user.UserId == userId).FirstOrDefaultAsync(photo => photo.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo ;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.User.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user ;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // tra ve users duoi dang co the query bang microsoft entity framwork core
            var users =  _context.User.Include(p => p.Photos)
                .OrderByDescending(user => user.LastActive).AsQueryable();

            users = users.Where(user => user.Id != userParams.UserId);

            users = users.Where(user => user.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(user => userLikers.Contains(user.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(user => userLikees.Contains(user.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created": 
                        users = users.OrderByDescending(user => user.Created);
                        break;
                    default:  
                        users = users.OrderByDescending(user => user.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(user => user.LikerId == userId && user.LikeeId == recipientId);
        }

        public async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.User.Include(user => user.Likers)
                .Include(user => user.Likees).FirstOrDefaultAsync(user => user.Id == id);

            if (likers)
            {
                return user.Likers.Where(user => user.LikeeId == id).Select(i => i.LikerId);
            } else {
                return user.Likees.Where(user => user.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(message => message.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(user => user.Sender)
                .ThenInclude(photo => photo.Photos)
                .Include(user => user.Recipient)
                .ThenInclude(photo => photo.Photos)
                .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId);
                    break;
                    case "Outbox":
                    messages = messages.Where(user => user.SenderId == messageParams.UserId);
                    break;
                    default:
                    messages = messages.Where(user => user.RecipientId == messageParams.UserId && user.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>
                            .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int useriD, int recipientId)
        {
            var messages = await _context.Messages
                .Include(user => user.Sender)
                .ThenInclude(photo => photo.Photos)
                .Include(user => user.Recipient)
                .ThenInclude(photo => photo.Photos)
                .Where(m => m.RecipientId == useriD && m.SenderId == recipientId
                    || m.RecipientId == recipientId && m.SenderId == useriD)
                .OrderByDescending(m => m.MessageSent) //order theo cai nao moi nhat
                .ToListAsync();

            return messages;
        }
    }
}