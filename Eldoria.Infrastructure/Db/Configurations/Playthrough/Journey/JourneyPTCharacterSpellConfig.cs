using Eldoria.Core.Entities.Playthrough.Journey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Journey;

public sealed class JourneyPTCharacterSpellConfig : IEntityTypeConfiguration<JourneyPTCharacterSpell>
{
    public void Configure(EntityTypeBuilder<JourneyPTCharacterSpell> builder)
    {
        builder.ToTable("JourneyPTCharacterSpells");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.JourneyPTCharacterId, x.PlaythroughSpellId }).IsUnique();

        builder.HasOne(x => x.JourneyPTCharacter)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.JourneyPTCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughSpell)
            .WithMany(x => x.JourneyCharacterSpells)
            .HasForeignKey(x => x.PlaythroughSpellId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
