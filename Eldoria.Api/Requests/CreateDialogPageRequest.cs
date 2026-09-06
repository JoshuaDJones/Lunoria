using System.ComponentModel.DataAnnotations;
using Eldoria.Core.Enums;

namespace Eldoria.Api.Requests;

public class CreateDialogPageRequest
{
    [Required]
    public int? OrderNum { get; set; }

    [Required]
    public DialogPageType? PageType { get; set; }

    [Required]
    public IFormFile Media { get; set; } = null!;
}
