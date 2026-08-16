using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class PTCharacterStatAdjustmentActionConfig
    : IEntityTypeConfiguration<PTCharacterStatAdjustmentAction>
{
    public void Configure(EntityTypeBuilder<PTCharacterStatAdjustmentAction> builder)
    {
        builder.ToTable("PTCharacterStatAdjustmentActions");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ScenePTActionEvent)
            .WithOne(x => x.CharacterStatAdjustmentAction)
            .HasForeignKey<PTCharacterStatAdjustmentAction>(x => x.ScenePTActionEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughCharacterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
