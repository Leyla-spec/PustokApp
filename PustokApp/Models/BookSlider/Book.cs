using PustokApp.Attributes;
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
        public string MainImageUrl { get; set; }
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
        [NotMapped]
        [FileLength(2)]
        [ContentType(["image/jpeg", "image/png", "image/jpg"])]
        public IFormFile MainPhoto { get; set; }
        [NotMapped]
        [FileLength(2)]
        [ContentType(["image/jpeg", "image/png", "image/jpg"])]
        public IFormFile HoverPhoto { get; set; }
        [NotMapped]
        [FileLength(2)]
        [ContentType(["image/jpeg", "image/png", "image/jpg"])]
        public IFormFile[] Photos { get; set; }
        public List <int> TagIds { get; set; }
        public Book()
        {
            BookImages = new List<BookImage>();
            BookTags = new List<BookTag>();
            TagIds = new List<int>();
        }
    }
    
}
