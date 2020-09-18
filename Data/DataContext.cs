//class thao tac voi co so du lieu
//sau 
using Microservices.Models;
using Microsoft.EntityFrameworkCore;

// moi khi co update code lien quan den CSDL nho migration, dotnet ef migrations add ...
namespace Microservices.Data
{
    public class DataContext : DbContext
    {
        // ham constructor 
        public DataContext(DbContextOptions<DataContext> options) : base (options){}

        // cai nay nhu kieu tao bang trong DB
        public DbSet<Value> Values { get; set; }        
        public DbSet<User> User { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        // khai bao bang co 2 khoa
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(key => new {
                    key.LikerId,
                    key.LikeeId
                });
            builder.Entity<Like>()
                .HasOne(user => user.Likee)
                .WithMany(user => user.Likers)
                .HasForeignKey(user => user.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
                .HasOne(user => user.Liker)
                .WithMany(user => user.Likees)
                .HasForeignKey(user => user.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(user => user.Sender)
                .WithMany(message => message.MessagesSent)  
                .OnDelete(DeleteBehavior.Restrict);

             builder.Entity<Message>()
                .HasOne(user => user.Recipient)
                .WithMany(message => message.MessagesReceived)  
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 