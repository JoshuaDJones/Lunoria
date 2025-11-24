using System.ComponentModel.DataAnnotations;

namespace Eldoria.BlazorClient.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters and include an uppercase letter, lowercase letter, number, and special character."
        )]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
