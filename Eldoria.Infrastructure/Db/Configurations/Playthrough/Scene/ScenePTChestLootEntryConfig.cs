using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTChestLootEntryConfig : IEntityTypeConfiguration<ScenePTChestLootEntry>
{
    public void Configure(EntityTypeBuilder<ScenePTChestLootEntry> builder)
    {
        builder.ToTable("ScenePTChestLootEntries", table =>
        {
            table.HasCheckConstraint(
                "CK_ScenePTChestLootEntries_RollRange",
                "[RollMinimum] >= 1 AND [RollMaximum] >= [RollMinimum]");
            table.HasCheckConstraint(
                "CK_ScenePTChestLootEntries_Quantity",
                "[Quantity] >= 1");
            table.HasCheckConstraint(
                "CK_ScenePTChestLootEntries_Item",
                "([PlaythroughEquippableItemId] IS NOT NULL AND [PlaythroughConsumableItemId] IS NULL) OR " +
                "([PlaythroughEquippableItemId] IS NULL AND [PlaythroughConsumableItemId] IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePTChestId, x.SourceSceneChestLootEntryId }).IsUnique();

        builder.HasOne(x => x.ScenePTChest)
            .WithMany(x => x.ChestLootEntries)
            .HasForeignKey(x => x.ScenePTChestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughEquippableItem)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughEquippableItemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PlaythroughConsumableItem)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughConsumableItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
