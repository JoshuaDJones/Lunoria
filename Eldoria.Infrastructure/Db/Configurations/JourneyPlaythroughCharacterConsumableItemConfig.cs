using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyPlaythroughCharacterConsumableItemConfig : IEntityTypeConfiguration<JourneyPlaythroughCharacterConsumableItem>
    {
        public void Configure(EntityTypeBuilder<JourneyPlaythroughCharacterConsumableItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.IsUsed).IsRequired();

            builder.HasOne(i => i.JourneyPlaythroughCharacter)
                .WithMany(c => c.ConsumableItems)
                .HasForeignKey(i => i.JourneyPlaythroughCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.ConsumableItem)
                .WithMany()
                .HasForeignKey(i => i.ConsumableItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
