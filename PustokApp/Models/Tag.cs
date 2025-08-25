using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class Tag : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}
