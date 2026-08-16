using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughConsumableItemConfig : IEntityTypeConfiguration<PlaythroughConsumableItem>
{
    public void Configure(EntityTypeBuilder<PlaythroughConsumableItem> builder)
    {
        builder.ToTable("PlaythroughConsumableItems");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceConsumableItemId }).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(250);
        builder.Property(x => x.PhotoUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.ConsumableItems)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
