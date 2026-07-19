using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughCharacterConfig : IEntityTypeConfiguration<ScenePlaythroughCharacter>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughCharacter> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.ScenePlaythroughId, c.SceneCharacterId })
                .IsUnique();

            builder.Property(c => c.Movement).IsRequired();
            builder.Property(c => c.MaxConsumableInventory).IsRequired();
            builder.Property(c => c.MaxEquippableInventory).IsRequired();
            builder.Property(c => c.CurrentHp).IsRequired();
            builder.Property(c => c.CurrentMp).IsRequired();
            builder.Property(c => c.MaxHp).IsRequired();
            builder.Property(c => c.MaxMp).IsRequired();
            builder.Property(c => c.IsDead).IsRequired();
            builder.Property(c => c.IsInAlternateForm).IsRequired();

            builder.HasOne(c => c.SceneCharacter)
                .WithMany()
                .HasForeignKey(c => c.SceneCharacterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.AlternateForm)
                .WithOne()
                .HasForeignKey<ScenePlaythroughCharacter>(c => c.AlternateFormId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
