using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneChestLootEntryConfig : IEntityTypeConfiguration<SceneChestLootEntry>
    {
        public void Configure(EntityTypeBuilder<SceneChestLootEntry> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.RollMinimum).IsRequired();
            builder.Property(e => e.RollMaximum).IsRequired();
            builder.Property(e => e.Quantity).IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_SceneChestLootEntries_RollRange",
                    "[RollMinimum] >= 1 AND [RollMaximum] >= [RollMinimum]");
                t.HasCheckConstraint(
                    "CK_SceneChestLootEntries_Quantity",
                    "[Quantity] >= 1");
                t.HasCheckConstraint(
                    "CK_SceneChestLootEntries_Item",
                    "([EquippableItemId] IS NOT NULL AND [ConsumableItemId] IS NULL) OR " +
                    "([EquippableItemId] IS NULL AND [ConsumableItemId] IS NOT NULL)");
            });

            builder.HasOne(e => e.EquippableItem)
                .WithMany()
                .HasForeignKey(e => e.EquippableItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.ConsumableItem)
                .WithMany()
                .HasForeignKey(e => e.ConsumableItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
