using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyPlaythroughCharacterSpellConfig : IEntityTypeConfiguration<JourneyPlaythroughCharacterSpell>
    {
        public void Configure(EntityTypeBuilder<JourneyPlaythroughCharacterSpell> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.JourneyPlaythroughCharacterId, s.JourneyCharacterSpellId })
                .IsUnique();

            builder.HasOne(s => s.JourneyPlaythroughCharacter)
                .WithMany(c => c.CharacterSpells)
                .HasForeignKey(s => s.JourneyPlaythroughCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.JourneyCharacterSpell)
                .WithMany()
                .HasForeignKey(s => s.JourneyCharacterSpellId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
