using Eldoria.Core.Enums;

namespace Eldoria.Api.Requests;

public class UpdateDialogPageRequest
{
    public int? OrderNum { get; set; }
    public DialogPageType? PageType { get; set; }
    public IFormFile? Media { get; set; }
}
