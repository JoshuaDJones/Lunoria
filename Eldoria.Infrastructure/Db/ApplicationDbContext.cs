using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<DialogPage> DialogPages { get; set; }
        public DbSet<DialogPageSection> DialogPageSections { get; set; }
        public DbSet<EquippableItem> EquippableItems { get; set; }

        public DbSet<Journey> Journeys { get; set; }
        public DbSet<JourneyCharacter> JourneyCharacters { get; set; }
        public DbSet<JourneyCharacterSpell> JourneyCharacterSpells { get; set; }
        public DbSet<JourneyIntroPage> JourneyIntroPages { get; set; }
        public DbSet<JourneyPlaythrough> JourneyPlaythroughs { get; set; }
        public DbSet<JourneyPlaythroughCharacter> JourneyPlaythroughCharacters { get; set; }
        public DbSet<JourneyPlaythroughCharacterConsumableItem> JourneyPlaythroughCharacterConsumableItems { get; set; }
        public DbSet<JourneyPlaythroughCharacterEquippableItem> JourneyPlaythroughCharacterEquippableItems { get; set; }
        public DbSet<JourneyPlaythroughCharacterSpell> JourneyPlaythroughCharacterSpells { get; set; }
        public DbSet<JourneyPlaythroughEventLog> JourneyPlaythroughEventLogs { get; set; }

        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SceneCharacter> SceneCharacters { get; set; }
        public DbSet<SceneCharacterSpell> SceneCharacterSpells { get; set; }
        public DbSet<SceneChest> SceneChests { get; set; }
        public DbSet<SceneChestLootEntry> SceneChestLootEntries { get; set; }
        public DbSet<SceneDialog> SceneDialogs { get; set; }
        public DbSet<SceneEvent> SceneEvents { get; set; }
        public DbSet<SceneEventAction> SceneEventActions { get; set; }
        public DbSet<SceneIntroPage> SceneIntroPages { get; set; }
        public DbSet<ScenePlaythrough> ScenePlaythroughs { get; set; }
        public DbSet<ScenePlaythroughCharacter> ScenePlaythroughCharacters { get; set; }
        public DbSet<ScenePlaythroughCharacterConsumableItem> ScenePlaythroughCharacterConsumableItems { get; set; }
        public DbSet<ScenePlaythroughCharacterEquippableItem> ScenePlaythroughCharacterEquippableItems { get; set; }
        public DbSet<ScenePlaythroughCharacterSpell> ScenePlaythroughCharacterSpells { get; set; }
        public DbSet<ScenePlaythroughChest> ScenePlaythroughChests { get; set; }
        public DbSet<ScenePlaythroughEvent> ScenePlaythroughEvents { get; set; }
        public DbSet<ScenePlaythroughParticipant> ScenePlaythroughParticipants { get; set; }

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
