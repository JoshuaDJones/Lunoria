using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneCharacterConfig : IEntityTypeConfiguration<SceneCharacter>
    {
        public void Configure(EntityTypeBuilder<SceneCharacter> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CurrentHp)
                .IsRequired();

            builder.Property(c => c.CurrentMp)
                .IsRequired();

            builder.Property(c => c.IsDown)
                .IsRequired();

            builder.Property(c => c.IsAlternateForm)
                .IsRequired();

            builder.HasOne(c => c.Character)
                   .WithMany()
                   .HasForeignKey(s => s.CharacterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.SceneCharacterItems)
                   .WithOne(i => i.SceneCharacter)
                   .HasForeignKey(i => i.SceneCharacterId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
