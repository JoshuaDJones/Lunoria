using Eldoria.Core.Entities;

namespace Eldoria.Application.Dtos
{
    public class SceneChestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DieSides { get; set; }
        public SceneDto Scene { get; set; } = null!;
        public List<SceneChestLootEntryDto> LootEntries { get; set; } = [];
    }
}
