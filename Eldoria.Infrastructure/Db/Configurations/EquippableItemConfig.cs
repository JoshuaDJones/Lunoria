using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class EquippableItemConfig : IEntityTypeConfiguration<EquippableItem>
    {
        public void Configure(EntityTypeBuilder<EquippableItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.UserId, e.Name })
                   .IsUnique();

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(e => e.PhotoUrl)
                   .IsRequired()
                   .HasMaxLength(2048);

            builder.Property(e => e.FileName)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(e => e.MeleeAttackDamageModifier)
                   .IsRequired();

            builder.Property(e => e.BowAttackDamageModifier)
                   .IsRequired();

            builder.Property(e => e.MovementModifier)
                   .IsRequired();

            builder.Property(e => e.MaxHpModifier)
                   .IsRequired();

            builder.Property(e => e.MaxMpModifier)
                   .IsRequired();

            builder.Property(e => e.MaxConsumableInventoryModifier)
                   .IsRequired();

            builder.Property(e => e.MaxEquippableInventoryModifier)
                   .IsRequired();

            builder.Property(e => e.MeleeDamageReduction)
                   .IsRequired();

            builder.Property(e => e.BowDamageReduction)
                   .IsRequired();

            builder.Property(e => e.SpellDamageReduction)
                   .IsRequired();

            builder.Property(e => e.SpellDamageModifier)
                   .IsRequired(false);

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.UpdatedAt)
                   .IsRequired();

            builder.HasOne(e => e.AffectedSpellType)
                   .WithMany(st => st.AffectedEquippableItems)
                   .HasForeignKey(e => e.AffectedSpellTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.AddedSpells)
                   .WithMany(s => s.EquippableItems)
                   .UsingEntity(j => j.ToTable("EquippableItemSpells"));
        }
    }
}
