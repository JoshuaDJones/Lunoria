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

            builder.Property(j => j.IsDown)
                .IsRequired();

            builder.Property(j => j.IsAlternateForm)
                .IsRequired();

            builder.HasOne(j => j.Character)
                   .WithMany()
                   .HasForeignKey(j => j.CharacterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.JourneyCharacterItems)
                   .WithOne(i => i.JourneyCharacter)
                   .HasForeignKey(j => j.JourneyCharacterId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
