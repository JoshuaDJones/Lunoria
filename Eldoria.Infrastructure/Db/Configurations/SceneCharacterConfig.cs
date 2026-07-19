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

            builder.HasIndex(c => new { c.SceneId, c.CharacterId })
                .IsUnique();

            builder.Property(c => c.Movement).IsRequired();
            builder.Property(c => c.MaxConsumableInventory).IsRequired();
            builder.Property(c => c.MaxEquippableInventory).IsRequired();
            builder.Property(c => c.MaxHp).IsRequired();
            builder.Property(c => c.MaxMp).IsRequired();
            builder.Property(c => c.IsInitiallyActive).IsRequired();

            builder.HasOne(c => c.Character)
                   .WithMany()
                   .HasForeignKey(s => s.CharacterId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.AlternateForm)
                .WithMany()
                .HasForeignKey(c => c.AlternateFormId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(c => !c.Character.IsDeleted);
        }
    }
}
