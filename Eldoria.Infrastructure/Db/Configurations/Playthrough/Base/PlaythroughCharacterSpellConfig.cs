using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughCharacterSpellConfig : IEntityTypeConfiguration<PlaythroughCharacterSpell>
{
    public void Configure(EntityTypeBuilder<PlaythroughCharacterSpell> builder)
    {
        builder.ToTable("PlaythroughCharacterSpells");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughCharacterId, x.PlaythroughSpellId }).IsUnique();

        builder.HasOne(x => x.PlaythroughCharacter)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.PlaythroughCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughSpell)
            .WithMany(x => x.BaseCharacterSpells)
            .HasForeignKey(x => x.PlaythroughSpellId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
