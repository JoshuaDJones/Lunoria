using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughCharacterSpellConfig : IEntityTypeConfiguration<ScenePlaythroughCharacterSpell>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughCharacterSpell> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.ScenePlaythroughCharacterId, s.SceneCharacterSpellId })
                .IsUnique();

            builder.HasOne(s => s.ScenePlaythroughCharacter)
                .WithMany(c => c.CharacterSpells)
                .HasForeignKey(s => s.ScenePlaythroughCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.SceneCharacterSpell)
                .WithMany()
                .HasForeignKey(s => s.SceneCharacterSpellId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
