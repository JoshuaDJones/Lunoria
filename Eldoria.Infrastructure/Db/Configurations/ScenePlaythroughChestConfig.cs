using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughChestConfig : IEntityTypeConfiguration<ScenePlaythroughChest>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughChest> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.ScenePlaythroughId, c.SceneChestId })
                .IsUnique();

            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.RolledValue).IsRequired(false);
            builder.Property(c => c.OpenedAt).IsRequired(false);

            builder.HasOne(c => c.SceneChest)
                .WithMany()
                .HasForeignKey(c => c.SceneChestId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.SelectedLootEntry)
                .WithMany()
                .HasForeignKey(c => c.SelectedLootEntryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
