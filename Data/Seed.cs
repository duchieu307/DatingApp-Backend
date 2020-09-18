using System.Collections.Generic;
using System.Linq;
using Microservices.Models;
using Newtonsoft.Json;

namespace Microservices.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if (!context.User.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.User.Add(user);
                }
                context.SaveChanges(); 
                
            }
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // https://xuanthulab.net/su-dung-giao-dien-idisposable-va-tu-khoa-using-trong-c-sharp.html
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            // giai phong tai nguyen khi code trong {} cua using chay xong
            {
                passwordSalt = hmac.Key;
                // ham ComputeHass nhan vao mot mang Byte nen phai chuyen password tu dang string sang dang byte[]
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }

        }
    }
}