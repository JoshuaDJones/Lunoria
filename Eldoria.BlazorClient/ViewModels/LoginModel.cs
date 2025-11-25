using System.ComponentModel.DataAnnotations;

namespace Eldoria.BlazorClient.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter Email Address.")]
        public string Email { get; set; } = "jdj92993@gmail.com";

        [Required(ErrorMessage = "Enter Password.")]
        public string Password { get; set; } = "Test123!";
    }
}
