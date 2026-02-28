using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class CharacterConfig : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.PhotoUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(c => c.FileName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.MaxHp)
                .IsRequired();

            builder.Property(c => c.MaxMp)
                .IsRequired();

            builder.Property(c => c.Movement)
                .IsRequired();

            builder.Property(c => c.MaxInventory)
                .IsRequired();

            builder.Property(c => c.IsPlayer)
                .IsRequired();

            builder.Property(c => c.IsNPC)
                .IsRequired();

            builder.Property(c => c.IsEnemy)
                .IsRequired();

            builder.Property(c => c.CreateDate)
                .IsRequired();

            builder.Property(c => c.UpdateDate)
                .IsRequired();

            // Soft delete configuration
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.DeletedAt)
                .IsRequired(false);

            // Global query filter to exclude soft-deleted characters
            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.HasOne(c => c.AlternateForm)
                   .WithOne()
                   .HasForeignKey<Character>(c => c.AlternateFormId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.CharacterSpells)
                   .WithOne(s => s.Character)
                   .HasForeignKey(c => c.CharacterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.CharacterDialogSettings)
                   .WithOne(d => d.Character)
                   .HasForeignKey<CharacterDialogSettings>(d => d.CharacterId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
