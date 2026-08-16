using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTGridConfig : IEntityTypeConfiguration<ScenePTGrid>
{
    public void Configure(EntityTypeBuilder<ScenePTGrid> builder)
    {
        builder.ToTable("ScenePTGrids", table =>
        {
            table.HasCheckConstraint("CK_ScenePTGrids_Rows", "[Rows] >= 1 AND [Rows] <= 100");
            table.HasCheckConstraint("CK_ScenePTGrids_Columns", "[Columns] >= 1 AND [Columns] <= 100");
        });
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ScenePTId).IsUnique();
        builder.Property(x => x.GridColor).IsRequired().HasMaxLength(7);
        builder.Property(x => x.BackgroundImageUrl).HasMaxLength(2048);
        builder.Property(x => x.BackgroundFileName).HasMaxLength(250);

        builder.HasOne(x => x.ScenePT)
            .WithOne(x => x.ScenePTGrid)
            .HasForeignKey<ScenePTGrid>(x => x.ScenePTId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
