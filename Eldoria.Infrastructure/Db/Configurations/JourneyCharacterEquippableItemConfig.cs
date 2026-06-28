using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyCharacterEquippableItemConfig : IEntityTypeConfiguration<JourneyCharacterEquippableItem>
    {
        public void Configure(EntityTypeBuilder<JourneyCharacterEquippableItem> builder)
        {
            builder.HasKey(jce => jce.Id);

            builder.Property(jce => jce.IsEquipped)
                   .IsRequired();

            builder.HasOne(jce => jce.JourneyCharacter)
                   .WithMany(jc => jc.JourneyCharacterEquippableItems)
                   .HasForeignKey(jce => jce.JourneyCharacterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(jce => jce.EquippableItem)
                   .WithMany(e => e.JourneyCharacterEquippableItems)
                   .HasForeignKey(jce => jce.EquippableItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(jce => !jce.JourneyCharacter.Character.IsDeleted);
        }
    }
}
