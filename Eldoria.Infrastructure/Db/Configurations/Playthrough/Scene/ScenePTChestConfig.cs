using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTChestConfig : IEntityTypeConfiguration<ScenePTChest>
{
    public void Configure(EntityTypeBuilder<ScenePTChest> builder)
    {
        builder.ToTable("ScenePTChests", table =>
        {
            table.HasCheckConstraint("CK_ScenePTChests_DieSides", "[DieSides] >= 1");
            table.HasCheckConstraint(
                "CK_ScenePTChests_RolledValue",
                "[RolledValue] IS NULL OR [RolledValue] >= 1");
        });
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePlaythroughId, x.SourceSceneChestId }).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.ScenePlaythrough)
            .WithMany(x => x.SceneChests)
            .HasForeignKey(x => x.ScenePlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SelectedLootEntry)
            .WithOne()
            .HasForeignKey<ScenePTChest>(x => x.SelectedLootEntryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
