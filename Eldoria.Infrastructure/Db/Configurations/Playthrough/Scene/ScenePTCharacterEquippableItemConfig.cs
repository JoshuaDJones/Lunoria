using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTCharacterEquippableItemConfig
    : IEntityTypeConfiguration<ScenePTCharacterEquippableItem>
{
    public void Configure(EntityTypeBuilder<ScenePTCharacterEquippableItem> builder)
    {
        builder.ToTable("ScenePTCharacterEquippableItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ScenePTCharacter)
            .WithMany(x => x.EquippableItems)
            .HasForeignKey(x => x.ScenePTCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughEquippableItem)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughEquippableItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
