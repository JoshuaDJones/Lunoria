using Eldoria.Core.Entities;

namespace Eldoria.Core.Entities.Playthrough.Base;

public sealed class PlaythroughStartAssets
{
    public required List<Character> Characters { get; init; }
    public required List<ConsumableItem> Consumables { get; init; }
    public required List<EquippableItem> Equippables { get; init; }
    public required List<Spell> Spells { get; init; }
    public required List<SpellType> SpellTypes { get; init; }
}
