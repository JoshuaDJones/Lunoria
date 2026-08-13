using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyPlaythroughCharacterConfig : IEntityTypeConfiguration<JourneyPlaythroughCharacter>
    {
        public void Configure(EntityTypeBuilder<JourneyPlaythroughCharacter> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.JourneyPlaythroughId, c.SnapshotAssignmentKey })
                .IsUnique();
            builder.Property(c => c.SnapshotCharacterKey).IsRequired().HasMaxLength(100);
            builder.Property(c => c.SnapshotAssignmentKey).IsRequired().HasMaxLength(100);

            builder.Property(c => c.Movement).IsRequired();
            builder.Property(c => c.MaxConsumableInventory).IsRequired();
            builder.Property(c => c.MaxEquippableInventory).IsRequired();
            builder.Property(c => c.CurrentHp).IsRequired();
            builder.Property(c => c.CurrentMp).IsRequired();
            builder.Property(c => c.MaxHp).IsRequired();
            builder.Property(c => c.MaxMp).IsRequired();
            builder.Property(c => c.IsDown).IsRequired();
            builder.Property(c => c.IsInAlternateForm).IsRequired();

            builder.HasOne(c => c.JourneyCharacter)
                .WithMany()
                .HasForeignKey(c => c.JourneyCharacterId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.AlternateForm)
                .WithOne()
                .HasForeignKey<JourneyPlaythroughCharacter>(c => c.AlternateFormId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
