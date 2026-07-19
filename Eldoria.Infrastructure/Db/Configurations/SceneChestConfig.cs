using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneChestConfig : IEntityTypeConfiguration<SceneChest>
    {
        public void Configure(EntityTypeBuilder<SceneChest> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.SceneId, c.Name })
                .IsUnique();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.DieSides).IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_SceneChests_DieSides",
                "[DieSides] >= 1"));

            builder.HasMany(c => c.LootEntries)
                .WithOne(e => e.SceneChest)
                .HasForeignKey(e => e.SceneChestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
