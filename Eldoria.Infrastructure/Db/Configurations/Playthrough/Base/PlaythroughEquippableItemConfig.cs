using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughEquippableItemConfig : IEntityTypeConfiguration<PlaythroughEquippableItem>
{
    public void Configure(EntityTypeBuilder<PlaythroughEquippableItem> builder)
    {
        builder.ToTable("PlaythroughEquippableItems");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceEquippableItemId }).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(250);
        builder.Property(x => x.PhotoUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.EquippableItems)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AffectedSpellType)
            .WithMany(x => x.AffectedEquippableItems)
            .HasForeignKey(x => x.AffectedSpellTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.AddedSpells)
            .WithMany(x => x.EquippableItems)
            .UsingEntity<Dictionary<string, object>>(
                "PlaythroughEquippableItemSpell",
                right => right.HasOne<PlaythroughSpell>()
                    .WithMany()
                    .HasForeignKey("PlaythroughSpellId")
                    .OnDelete(DeleteBehavior.NoAction),
                left => left.HasOne<PlaythroughEquippableItem>()
                    .WithMany()
                    .HasForeignKey("PlaythroughEquippableItemId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("PlaythroughEquippableItemSpells");
                    join.HasKey("PlaythroughEquippableItemId", "PlaythroughSpellId");
                });
    }
}
