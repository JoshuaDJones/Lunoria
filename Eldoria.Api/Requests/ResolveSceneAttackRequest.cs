using System.ComponentModel.DataAnnotations;
using Eldoria.Core.Enums;

namespace Eldoria.Api.Requests;

public sealed class ResolveSceneAttackRequest
{
    [Range(1, int.MaxValue)]
    public int TargetParticipantId { get; set; }

    [EnumDataType(typeof(SceneAttackType))]
    public SceneAttackType AttackType { get; set; }

    [Range(1, 6)]
    public int Roll { get; set; }

    [Range(1, int.MaxValue)]
    public int? PlaythroughSpellId { get; set; }
}
