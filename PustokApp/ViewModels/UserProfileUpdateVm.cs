using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class UserProfileUpdateVm
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        [EmailAddress,DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
