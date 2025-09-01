using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PustokApp.Models.BookSlider;
using PustokApp.Models.Common;

namespace PustokApp.Models
{
    public class Slider : AuditEntity
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Btntext { get; set; }
        public string BtnLink { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
