namespace Eldoria.Api.Requests
{
    public class UpdateSpellRequest
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public IFormFile? Photo { get; set; }
        public int Range { get; set; }
        public bool IsRadius { get; set; }
        public int MpCost { get; set; }
        public int? DamageEffect { get; set; }
        public int? HealthEffect { get; set; }
        public int? MagicEffect { get; set; }
    }
}
