using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebScheduler.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).*$", ErrorMessage = "The {0} format is invalid.")]
        [Display(Name = "password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Compare("Password", ErrorMessage = "The password confirmation does not match.")]
        [Display(Name = "repeat password")]
        public string RepeatPassword { get; set; }
    }
}
