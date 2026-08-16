using Eldoria.Core.Entities.Playthrough.Journey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Journey;

public sealed class JourneyPTCharacterEquippableItemConfig
    : IEntityTypeConfiguration<JourneyPTCharacterEquippableItem>
{
    public void Configure(EntityTypeBuilder<JourneyPTCharacterEquippableItem> builder)
    {
        builder.ToTable("JourneyPTCharacterEquippableItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.JourneyPTCharacter)
            .WithMany(x => x.EquippableItems)
            .HasForeignKey(x => x.JourneyPTCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughEquippableItem)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughEquippableItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
