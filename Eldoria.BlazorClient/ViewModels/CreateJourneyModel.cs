using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace Eldoria.BlazorClient.ViewModels
{
    public class CreateJourneyModel
    {
        [Required(ErrorMessage = "Name Required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description Required.")]
        public string Description { get; set; } = string.Empty;

        [RequiredIfMissing("NewPhoto", ErrorMessage = "You must provide a photo.")]
        public string? ExistingPhotoUrl { get; set; }

        [RequiredIfMissing("ExistingPhotoUrl", ErrorMessage = "You must provide a photo.")]
        public IBrowserFile? NewPhoto { get; set; }
    }
}
