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

            builder.Property(j => j.CurrentHp)
                .IsRequired();

            builder.Property(j => j.CurrentMp)
                .IsRequired();

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

            builder.Property(j => j.IsDown)
                .IsRequired();

            builder.Property(j => j.IsInAlternateForm)
                .IsRequired();

            builder.HasOne(j => j.Character)
                   .WithMany()
                   .HasForeignKey(j => j.CharacterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.AlternateForm)
                   .WithMany()
                   .HasForeignKey(j => j.AlternateFormId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.JourneyCharacterItems)
                   .WithOne(i => i.JourneyCharacter)
                   .HasForeignKey(j => j.JourneyCharacterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(j => !j.Character.IsDeleted);
        }
    }
}
