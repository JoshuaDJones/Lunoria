namespace Eldoria.Application.Dtos
{
    public class SpellDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public int Range { get; set; }
        public bool IsRadius { get; set; }
        public int MpCost { get; set; }
        public int? DamageEffect { get; set; }
        public int? HealthEffect { get; set; }
        public int? MagicEffect { get; set; }
        public DateTime CreateDate { get; set; }
        public int SpellTypeId { get; set; }
        public SpellTypeDto? SpellType { get; set; }
    }
}
