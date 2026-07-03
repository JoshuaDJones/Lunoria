using Eldoria.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSceneProgressStatusRequest
    {
        [Required]
        [EnumDataType(typeof(SceneProgressStatus))]
        public SceneProgressStatus? Status { get; set; }
    }
}
