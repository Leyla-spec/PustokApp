using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokApp.Models.BookSlider
{
    public class Book : AuditEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name can not be longer than 50 chars")]
        public string Title { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name can not be longer than 100 chars")]
        public string Description { get; set; }
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public List<BookImage> BookImages { get; set; }
        [Required]
        public string MainImageUrl { get; set; }
        [Required]
        public string HoverImageUrl { get; set; }
        public Genres Genre { get; set; }
        public int GenreId { get; set; }
        [Column (TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public bool InStock { get; set; }
        public string Code { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}
