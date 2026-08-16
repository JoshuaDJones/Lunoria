using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTCharacterConsumableItemConfig
    : IEntityTypeConfiguration<ScenePTCharacterConsumableItem>
{
    public void Configure(EntityTypeBuilder<ScenePTCharacterConsumableItem> builder)
    {
        builder.ToTable("ScenePTCharacterConsumableItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ScenePTCharacter)
            .WithMany(x => x.ConsumableItems)
            .HasForeignKey(x => x.ScenePTCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughConsumableItem)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughConsumableItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
