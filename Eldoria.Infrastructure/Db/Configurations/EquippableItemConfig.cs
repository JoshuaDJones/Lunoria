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

            builder.Property(e => e.CreateDate)
                   .IsRequired();

            builder.Property(e => e.UpdateDate)
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
