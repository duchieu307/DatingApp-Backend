namespace Microservices.Models
{
    public class Like
    {
        public int LikerId { get; set; } //nguoi like
        public int LikeeId { get; set; } //nguoi dc like
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}