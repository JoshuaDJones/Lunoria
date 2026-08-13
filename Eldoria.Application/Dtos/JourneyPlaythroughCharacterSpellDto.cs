namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterSpellDto
    {
        public int Id { get; set; }
        public int JourneyPlaythroughCharacterId { get; set; }
        public int? SourceJourneyCharacterSpellId { get; set; }
        public string SnapshotSpellKey { get; set; } = string.Empty;
    }
}
