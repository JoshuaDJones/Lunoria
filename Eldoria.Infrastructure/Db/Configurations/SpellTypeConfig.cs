using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SpellTypeConfig : IEntityTypeConfiguration<SpellType>
    {
        public void Configure(EntityTypeBuilder<SpellType> builder)
        {
            builder.HasKey(st => st.Id);

            builder.Property(st => st.TypeName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(st => st.Description)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(st => st.PhotoUrl)
                   .IsRequired()
                   .HasMaxLength(2048);

            builder.Property(st => st.FileName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(st => new { st.UserId, st.TypeName })
                   .IsUnique();
        }
    }
}
