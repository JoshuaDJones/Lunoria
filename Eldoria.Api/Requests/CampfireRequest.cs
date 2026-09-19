using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests;

public sealed class CampfireRequest
{
    [Required, RegularExpression("^(hp|mp)$")]
    public string Resource { get; set; } = string.Empty;
}
