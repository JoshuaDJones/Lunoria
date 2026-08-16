using Eldoria.Core.Enums;

using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughCharacter
    {
        public int Id { get; set; }
        public int SourceCharacterId { get; set; }
        public CharacterType CharacterType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? PortraitUrl { get; set; } = string.Empty;
        public string? PortraitFileName { get; set; } = string.Empty;

        public int BaseMaxHp { get; set; }
        public int BaseMaxMp { get; set; }
        public int? BaseMeleeAttackDamage { get; set; }
        public int? BaseBowAttackDamage { get; set; }
        public int BaseMovement { get; set; }
        public int BaseMaxConsumableInventory { get; set; }
        public int BaseMaxEquippableInventory { get; set; }

        public int? BaseAlternateFormId { get; set; }
        public PlaythroughCharacter? BaseAlternateForm { get; set; } = null!;

        public string DialogActiveColor { get; set; } = string.Empty;
        public string DialogInActiveColor { get; set; } = string.Empty;

        public int PlaythroughId { get; set; }
        public Playthrough Playthrough { get; set; } = null!;

        public ICollection<PlaythroughCharacterSpell> Spells { get; set; } = [];
        public ICollection<ScenePTDialogSection> DialogSections { get; set; } = [];
    }
}
