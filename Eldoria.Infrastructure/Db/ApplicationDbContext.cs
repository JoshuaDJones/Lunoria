using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using PTDialogPage = Eldoria.Core.Entities.Playthrough.Scene.ScenePTDialogPage;
using PTDialogSection = Eldoria.Core.Entities.Playthrough.Scene.ScenePTDialogSection;

namespace Eldoria.Infrastructure.Db
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterDialogSettings> CharacterDialogSettings { get; set; }
        public DbSet<CharacterSpell> CharacterSpells { get; set; }
        public DbSet<CharacterStatAdjustmentAction> CharacterStatAdjustmentActions { get; set; }
        public DbSet<ConsumableItem> ConsumableItems { get; set; }
        public DbSet<EquippableItem> EquippableItems { get; set; }

        public DbSet<Journey> Journeys { get; set; }
        public DbSet<JourneyCharacter> JourneyCharacters { get; set; }
        public DbSet<JourneyCharacterSpell> JourneyCharacterSpells { get; set; }
        public DbSet<JourneyIntroPage> JourneyIntroPages { get; set; }

        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SceneCharacter> SceneCharacters { get; set; }
        public DbSet<SceneCharacterSpell> SceneCharacterSpells { get; set; }
        public DbSet<SceneChest> SceneChests { get; set; }
        public DbSet<SceneChestLootEntry> SceneChestLootEntries { get; set; }
        public DbSet<SceneDialog> SceneDialogs { get; set; }
        public DbSet<SceneEvent> SceneEvents { get; set; }
        public DbSet<SceneEventAction> SceneEventActions { get; set; }
        public DbSet<SceneGrid> SceneGrids { get; set; }
        public DbSet<SceneIntroPage> SceneIntroPages { get; set; }

        public DbSet<Eldoria.Core.Entities.Playthrough.Base.Playthrough> Playthroughs { get; set; }
        public DbSet<PlaythroughCharacter> PlaythroughCharacters { get; set; }
        public DbSet<PlaythroughCharacterSpell> PlaythroughCharacterSpells { get; set; }
        public DbSet<PlaythroughConsumableItem> PlaythroughConsumableItems { get; set; }
        public DbSet<PlaythroughEquippableItem> PlaythroughEquippableItems { get; set; }
        public DbSet<PlaythroughEventLog> PlaythroughEventLogs { get; set; }
        public DbSet<PlaythroughIntroPage> PlaythroughIntroPages { get; set; }
        public DbSet<PlaythroughSpell> PlaythroughSpells { get; set; }
        public DbSet<PlaythroughSpellType> PlaythroughSpellTypes { get; set; }

        public DbSet<JourneyPTCharacter> JourneyPTCharacters { get; set; }
        public DbSet<JourneyPTCharacterSpell> JourneyPTCharacterSpells { get; set; }
        public DbSet<JourneyPTCharacterConsumableItem> JourneyPTCharacterConsumableItems { get; set; }
        public DbSet<JourneyPTCharacterEquippableItem> JourneyPTCharacterEquippableItems { get; set; }

        public DbSet<ScenePT> ScenePTs { get; set; }
        public DbSet<ScenePTGrid> ScenePTGrids { get; set; }
        public DbSet<ScenePTIntroPage> ScenePTIntroPages { get; set; }
        public DbSet<ScenePTCharacter> ScenePTCharacters { get; set; }
        public DbSet<ScenePTCharacterSpell> ScenePTCharacterSpells { get; set; }
        public DbSet<ScenePTCharacterConsumableItem> ScenePTCharacterConsumableItems { get; set; }
        public DbSet<ScenePTCharacterEquippableItem> ScenePTCharacterEquippableItems { get; set; }
        public DbSet<ScenePTParticipant> ScenePTParticipants { get; set; }
        public DbSet<ScenePTChest> ScenePTChests { get; set; }
        public DbSet<ScenePTChestLootEntry> ScenePTChestLootEntries { get; set; }
        public DbSet<ScenePTDialog> ScenePTDialogs { get; set; }
        public DbSet<PTDialogPage> ScenePTDialogPages { get; set; }
        public DbSet<PTDialogSection> ScenePTDialogSections { get; set; }
        public DbSet<ScenePTEvent> ScenePTEvents { get; set; }
        public DbSet<ScenePTActionEvent> ScenePTActionEvents { get; set; }
        public DbSet<PTCharacterStatAdjustmentAction> PTCharacterStatAdjustmentActions { get; set; }
        public DbSet<PTCharacterAddSpellAction> PTCharacterAddSpellActions { get; set; }

        public DbSet<Series> Series { get; set; }
        public DbSet<Spell> Spells { get; set; }
        public DbSet<SpellType> SpellTypes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
