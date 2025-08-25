using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class Slider : AuditEntity
    {
        public int Id { get; set; }
        [Required]
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
    }
}
