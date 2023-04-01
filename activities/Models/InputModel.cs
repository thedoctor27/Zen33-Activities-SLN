using activities.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace activities.Models
{
    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [MaxLength(200)]
        [Required]
        public string Name { get; set; }
        [MaxLength(100)]
        [Required]
        public string City { get; set; }
        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 40)]
        public string About { get; set; }
        [Required]
        public int IdActivity { get; set; }
        [Required]
        public int IdCountry { get; set; }
        [Required]
        public int IdLanguage { get; set; }

        public bool Available { get; set; }

        [DataType(DataType.Upload)]
        [MaxFileSize(10 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile? Photo { get; set; }
        public string? Base64Photo { get; set; }


        public string? Approval { get; set; }
        public string? ApprovalMessage { get; set; }
        public string? Member { get; set; }
    }
}
