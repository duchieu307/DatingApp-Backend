using System;
using System.Threading.Tasks;
using Microservices.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.User.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return  user; 
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // dung passwordSalt cua nguoi dung nhu la khoa, de tinh toan ra passwordHash roi so sanh
                // voi passwordHash trong DB
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;  
                    }
                }

            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            // khi passwordHash va passwordSalt dc update trong ham` CreatePassword, no se dc update luon 
            // vao cac bien khoi tao nno
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // https://xuanthulab.net/su-dung-giao-dien-idisposable-va-tu-khoa-using-trong-c-sharp.html
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            // giai phong tai nguyen khi code trong {} cua using chay xong
            {
                passwordSalt = hmac.Key;
                // ham ComputeHass nhan vao mot mang Byte nen phai chuyen password tu dang string sang dang byte[]
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.User.AnyAsync(x => x.Username == username)){
                return true;
            }
            return false;
        }

        
    }
}