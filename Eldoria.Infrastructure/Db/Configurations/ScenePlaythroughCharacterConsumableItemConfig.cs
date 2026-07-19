using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughCharacterConsumableItemConfig : IEntityTypeConfiguration<ScenePlaythroughCharacterConsumableItem>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughCharacterConsumableItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.IsUsed).IsRequired();

            builder.HasOne(i => i.ScenePlaythroughCharacter)
                .WithMany(c => c.ConsumableItems)
                .HasForeignKey(i => i.ScenePlaythroughCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.ConsumableItem)
                .WithMany()
                .HasForeignKey(i => i.ConsumableItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
