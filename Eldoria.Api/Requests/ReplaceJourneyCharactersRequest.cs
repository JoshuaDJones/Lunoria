using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class ReplaceJourneyCharactersRequest
    {
        [Required]
        public List<int> CharacterIds { get; set; } = new();
    }

    public class UpdateJourneyCharacterStatsRequest
    {
        [Range(0, int.MaxValue)] public int? MeleeAttackDamage { get; set; }
        [Range(0, int.MaxValue)] public int? BowAttackDamage { get; set; }
        [Required, Range(0, int.MaxValue)] public int? Movement { get; set; }
        [Required, Range(0, int.MaxValue)] public int? MaxConsumableInventory { get; set; }
        [Required, Range(0, int.MaxValue)] public int? MaxEquippableInventory { get; set; }
        [Required, Range(1, int.MaxValue)] public int? MaxHp { get; set; }
        [Required, Range(0, int.MaxValue)] public int? MaxMp { get; set; }
        [Required] public bool? IsInitiallyActive { get; set; }
        [Range(1, int.MaxValue)] public int? AlternateFormId { get; set; }
    }
}
