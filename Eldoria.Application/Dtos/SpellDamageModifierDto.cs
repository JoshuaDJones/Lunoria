namespace Eldoria.Application.Dtos
{
    public class SpellDamageModifierDto
    {
        public int? SpellTypeId { get; set; }
        public string? SpellTypeName { get; set; }
        public int Modifier { get; set; }
    }
}
