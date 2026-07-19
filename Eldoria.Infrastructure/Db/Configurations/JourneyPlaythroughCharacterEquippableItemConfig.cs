using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyPlaythroughCharacterEquippableItemConfig : IEntityTypeConfiguration<JourneyPlaythroughCharacterEquippableItem>
    {
        public void Configure(EntityTypeBuilder<JourneyPlaythroughCharacterEquippableItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.JourneyPlaythroughCharacter)
                .WithMany(c => c.EquippableItems)
                .HasForeignKey(i => i.JourneyPlaythroughCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.EquippableItem)
                .WithMany()
                .HasForeignKey(i => i.EquippableItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
