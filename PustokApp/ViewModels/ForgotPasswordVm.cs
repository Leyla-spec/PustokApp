using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class ForgotPasswordVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
