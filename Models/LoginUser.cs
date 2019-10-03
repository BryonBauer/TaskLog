using System.ComponentModel.DataAnnotations;

namespace TaskLog.Models
{
    public class LoginUser
    {
        // No other fields!
        [Required]
        [Display(Name="Email")]
        public string LoginEmail { get; set; }

        [Required]
        [Display(Name="Password")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string LoginPassword { get; set; }
    }
}