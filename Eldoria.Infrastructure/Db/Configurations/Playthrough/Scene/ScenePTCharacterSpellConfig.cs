using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTCharacterSpellConfig : IEntityTypeConfiguration<ScenePTCharacterSpell>
{
    public void Configure(EntityTypeBuilder<ScenePTCharacterSpell> builder)
    {
        builder.ToTable("ScenePTCharacterSpells");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePTCharacterId, x.PlaythroughSpellId }).IsUnique();

        builder.HasOne(x => x.ScenePTCharacter)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.ScenePTCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughSpell)
            .WithMany(x => x.SceneCharacterSpells)
            .HasForeignKey(x => x.PlaythroughSpellId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
