using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyCharacterConfig : IEntityTypeConfiguration<JourneyCharacter>
    {
        public void Configure(EntityTypeBuilder<JourneyCharacter> builder)
        {
            builder.HasKey(j => j.Id);

            builder.HasIndex(j => new { j.JourneyId, j.CharacterId })
                .IsUnique();

            builder.Property(j => j.MaxHp)
                .IsRequired();

            builder.Property(j => j.MaxMp)
                .IsRequired();

            builder.Property(j => j.Movement)
                .IsRequired();

            builder.Property(j => j.MaxConsumableInventory)
                .IsRequired();

            builder.Property(j => j.MaxEquippableInventory)
                .IsRequired();

            builder.Property(j => j.IsInitiallyActive)
                .IsRequired();

            builder.HasOne(j => j.Character)
                   .WithMany()
                   .HasForeignKey(j => j.CharacterId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(j => j.AlternateForm)
                   .WithMany()
                   .HasForeignKey(j => j.AlternateFormId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(j => !j.Character.IsDeleted);
        }
    }
}
