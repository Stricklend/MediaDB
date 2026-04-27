using System.ComponentModel.DataAnnotations;

namespace MediaDB.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "ID is required.")]
        [StringLength(50, ErrorMessage = "ID must be 50 characters or less.")]
        [Display(Name = "ID")]
        public string user_id { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, ErrorMessage = "Password must be 255 characters or less.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string user_password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [StringLength(255, ErrorMessage = "Password confirmation must be 255 characters or less.")]
        [DataType(DataType.Password)]
        [Compare("user_password", ErrorMessage = "Password and confirmation do not match.")]
        [Display(Name = "Confirm Password")]
        public string confirm_user_password { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username must be 50 characters or less.")]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email must be 100 characters or less.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string email { get; set; }
    }
}
