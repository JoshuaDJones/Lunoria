using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SpellConfig : IEntityTypeConfiguration<Spell>
    {
        public void Configure(EntityTypeBuilder<Spell> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.PhotoUrl)
                .HasMaxLength(2048);

            builder.Property(s => s.FileName)
                .HasMaxLength(250);

            builder.Property(s => s.Range)
                .IsRequired();

            builder.Property(s => s.IsRadius)
                .IsRequired();

            builder.Property(s => s.MpCost)
                .IsRequired();

            builder.Property(c => c.CreateDate)
                .IsRequired();

            builder.Property(c => c.UpdateDate)
                .IsRequired();

            builder.HasOne(s => s.SpellType)
                   .WithMany(st => st.Spells)
                   .HasForeignKey(s => s.SpellTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
