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

            builder.HasIndex(st => new { st.UserId, st.TypeName })
                   .IsUnique();
        }
    }
}
