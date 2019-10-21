namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // chi mette like
        public int LikeeId { get; set; } // chi lo riceve
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}