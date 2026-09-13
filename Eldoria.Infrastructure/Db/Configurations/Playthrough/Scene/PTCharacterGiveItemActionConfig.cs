using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class PTCharacterGiveItemActionConfig : IEntityTypeConfiguration<PTCharacterGiveItemAction>
{
    public void Configure(EntityTypeBuilder<PTCharacterGiveItemAction> builder)
    {
        builder.ToTable("PTCharacterGiveItemActions", table =>
        {
            table.HasCheckConstraint("CK_PTCharacterGiveItemAction_Item", "([PlaythroughConsumableItemId] IS NULL AND [PlaythroughEquippableItemId] IS NOT NULL) OR ([PlaythroughConsumableItemId] IS NOT NULL AND [PlaythroughEquippableItemId] IS NULL)");
            table.HasCheckConstraint("CK_PTCharacterGiveItemAction_Quantity", "[Quantity] BETWEEN 1 AND 1000");
        });
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.ScenePTActionEvent).WithOne(x => x.CharacterGiveItemAction)
            .HasForeignKey<PTCharacterGiveItemAction>(x => x.ScenePTActionEventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.PlaythroughCharacter).WithMany()
            .HasForeignKey(x => x.PlaythroughCharacterId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.PlaythroughConsumableItem).WithMany()
            .HasForeignKey(x => x.PlaythroughConsumableItemId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.PlaythroughEquippableItem).WithMany()
            .HasForeignKey(x => x.PlaythroughEquippableItemId).OnDelete(DeleteBehavior.NoAction);
    }
}
