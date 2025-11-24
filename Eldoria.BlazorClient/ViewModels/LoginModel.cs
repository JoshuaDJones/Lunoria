using System.ComponentModel.DataAnnotations;

namespace Eldoria.BlazorClient.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter Email Address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Password.")]
        public string Password { get; set; } = string.Empty;
    }
}
