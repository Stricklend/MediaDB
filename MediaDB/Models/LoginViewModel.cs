using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MediaDB.Models
{
    public class LoginViewModel
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
       
        [StringLength(255, ErrorMessage = "Password confirmation must be 255 characters or less.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password 확인")]
        public string confirm_user_password { get; set; }
    }
}
