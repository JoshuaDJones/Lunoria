using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class PTCharacterAddSpellActionConfig : IEntityTypeConfiguration<PTCharacterAddSpellAction>
{
    public void Configure(EntityTypeBuilder<PTCharacterAddSpellAction> builder)
    {
        builder.ToTable("PTCharacterAddSpellActions");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ScenePTActionEvent)
            .WithOne(x => x.CharacterAddSpellAction)
            .HasForeignKey<PTCharacterAddSpellAction>(x => x.ScenePTActionEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughCharacter)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughCharacterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PlaythroughSpell)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughSpellId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
