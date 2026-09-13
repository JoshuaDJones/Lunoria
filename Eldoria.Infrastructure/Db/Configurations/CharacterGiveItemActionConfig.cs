using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class CharacterGiveItemActionConfig : IEntityTypeConfiguration<CharacterGiveItemAction>
{
    public void Configure(EntityTypeBuilder<CharacterGiveItemAction> builder)
    {
        builder.ToTable("CharacterGiveItemActions", table =>
        {
            table.HasCheckConstraint("CK_CharacterGiveItemAction_Item", "([ConsumableItemId] IS NULL AND [EquippableItemId] IS NOT NULL) OR ([ConsumableItemId] IS NOT NULL AND [EquippableItemId] IS NULL)");
            table.HasCheckConstraint("CK_CharacterGiveItemAction_Quantity", "[Quantity] BETWEEN 1 AND 1000");
        });
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.SceneEventAction).WithOne(x => x.CharacterGiveItemAction)
            .HasForeignKey<CharacterGiveItemAction>(x => x.SceneEventActionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Character).WithMany()
            .HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.ConsumableItem).WithMany()
            .HasForeignKey(x => x.ConsumableItemId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.EquippableItem).WithMany()
            .HasForeignKey(x => x.EquippableItemId).OnDelete(DeleteBehavior.NoAction);
    }
}
