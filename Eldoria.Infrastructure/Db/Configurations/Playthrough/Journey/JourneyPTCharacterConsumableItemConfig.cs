using Eldoria.Core.Entities.Playthrough.Journey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Journey;

public sealed class JourneyPTCharacterConsumableItemConfig
    : IEntityTypeConfiguration<JourneyPTCharacterConsumableItem>
{
    public void Configure(EntityTypeBuilder<JourneyPTCharacterConsumableItem> builder)
    {
        builder.ToTable("JourneyPTCharacterConsumableItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.JourneyPTCharacter)
            .WithMany(x => x.ConsumableItems)
            .HasForeignKey(x => x.JourneyPTCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughConsumableItem)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughConsumableItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
