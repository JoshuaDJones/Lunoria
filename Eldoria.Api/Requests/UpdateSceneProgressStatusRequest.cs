using Eldoria.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSceneProgressStatusRequest
    {
        [Required]
        [EnumDataType(typeof(ScenePlaythroughStatus))]
        public ScenePlaythroughStatus? Status { get; set; }
    }
}
