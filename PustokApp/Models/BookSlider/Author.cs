using PustokApp.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models.BookSlider
{
    public class Author : BaseEntity
    {
        [Required]
        [StringLength(50, ErrorMessage ="Name can not be longer than 50 chars")]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
