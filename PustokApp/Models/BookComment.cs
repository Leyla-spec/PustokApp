using PustokApp.Models.BookSlider;

namespace PustokApp.Models
{
    public class BookComment
    {
        public int Id { get; set; }
        public string ontent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public CommentStatus Status { get; set; } = CommentStatus.Pending;
        public int Rate { get; set; }

    }
    public enum CommentStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
