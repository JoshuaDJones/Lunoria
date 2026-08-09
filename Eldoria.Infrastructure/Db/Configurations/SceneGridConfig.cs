using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneGridConfig : IEntityTypeConfiguration<SceneGrid>
    {
        public void Configure(EntityTypeBuilder<SceneGrid> builder)
        {
            builder.ToTable(table =>
            {
                table.HasCheckConstraint("CK_SceneGrids_Rows", "[Rows] >= 1 AND [Rows] <= 100");
                table.HasCheckConstraint("CK_SceneGrids_Columns", "[Columns] >= 1 AND [Columns] <= 100");
            });

            builder.HasKey(grid => grid.Id);

            builder.HasIndex(grid => grid.SceneId)
                .IsUnique();

            builder.Property(grid => grid.Rows)
                .IsRequired();

            builder.Property(grid => grid.Columns)
                .IsRequired();

            builder.Property(grid => grid.GridColor)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(grid => grid.BackgroundImageUrl)
                .IsRequired(false)
                .HasMaxLength(2048);

            builder.Property(grid => grid.BackgroundFileName)
                .IsRequired(false)
                .HasMaxLength(250);

            builder.Property(grid => grid.CreatedAt)
                .IsRequired();

            builder.Property(grid => grid.UpdatedAt)
                .IsRequired();

            builder.HasOne(grid => grid.Scene)
                .WithOne(scene => scene.Grid)
                .HasForeignKey<SceneGrid>(grid => grid.SceneId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
