namespace Eldoria.Core.Entities
{
    public class SceneChest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DieSides { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public ICollection<SceneChestLootEntry> LootEntries { get; set; } = [];
    }
}
