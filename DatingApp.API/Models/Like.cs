namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // chi mette like
        public int LikeeId { get; set; } // chi lo riceve
        public virtual User Liker { get; set; }
        public virtual User Likee { get; set; }
    }
}