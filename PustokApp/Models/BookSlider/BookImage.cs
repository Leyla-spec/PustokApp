using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models.BookSlider
{
    public class BookImage : BaseEntity
    {
        [Required]
        public string ImageUrl { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
