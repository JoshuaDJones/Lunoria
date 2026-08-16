namespace Eldoria.Core.Entities
{
    public class CharacterAddSpellAction
    {
        public int Id { get; set; }
        
        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int SpellId { get; set; }
        public Spell Spell { get; set; } = null!;

        public int SceneEventActionId { get; set; }
        public SceneEventAction SceneEventAction { get; set; } = null!;
    }
}
