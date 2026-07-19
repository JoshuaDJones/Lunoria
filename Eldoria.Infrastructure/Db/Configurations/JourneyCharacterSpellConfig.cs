using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyCharacterSpellConfig : IEntityTypeConfiguration<JourneyCharacterSpell>
    {
        public void Configure(EntityTypeBuilder<JourneyCharacterSpell> builder)
        {
            builder.HasKey(jcs => jcs.Id);

            builder.HasIndex(jcs => new { jcs.JourneyCharacterId, jcs.SpellId })
                   .IsUnique();

            builder.HasOne(jcs => jcs.JourneyCharacter)
                   .WithMany(jc => jc.JourneyCharacterSpells)
                   .HasForeignKey(jcs => jcs.JourneyCharacterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(jcs => jcs.Spell)
                   .WithMany(s => s.JourneyCharacterSpells)
                   .HasForeignKey(jcs => jcs.SpellId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(jcs => !jcs.JourneyCharacter.Character.IsDeleted);
        }
    }
}
