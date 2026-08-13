using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughCharacterEquippableItemConfig : IEntityTypeConfiguration<ScenePlaythroughCharacterEquippableItem>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughCharacterEquippableItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.SnapshotEquipmentKey).IsRequired().HasMaxLength(100);
            builder.Property(i => i.IsEquipped).IsRequired();

            builder.HasOne(i => i.ScenePlaythroughCharacter)
                .WithMany(c => c.EquippableItems)
                .HasForeignKey(i => i.ScenePlaythroughCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.EquippableItem)
                .WithMany()
                .HasForeignKey(i => i.EquippableItemId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
