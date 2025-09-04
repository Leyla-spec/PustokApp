using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class UserLoginVm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
