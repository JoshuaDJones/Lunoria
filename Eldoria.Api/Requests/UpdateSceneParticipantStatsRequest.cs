using System.ComponentModel.DataAnnotations;
using Eldoria.Application.Dtos;

namespace Eldoria.Api.Requests;

public sealed class UpdateSceneParticipantStatsRequest
{
    [Range(0, int.MaxValue)]
    public int CurrentHp { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxHp { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentMp { get; set; }

    [Range(0, int.MaxValue)]
    public int MaxMp { get; set; }

    public int Movement { get; set; }

    public int? MeleeAttackDamage { get; set; }

    public int? BowAttackDamage { get; set; }

    public SceneParticipantStatsUpdateDto ToDto() => new()
    {
        CurrentHp = CurrentHp,
        MaxHp = MaxHp,
        CurrentMp = CurrentMp,
        MaxMp = MaxMp,
        Movement = Movement,
        MeleeAttackDamage = MeleeAttackDamage,
        BowAttackDamage = BowAttackDamage
    };
}
