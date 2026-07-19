using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SeriesConfig : IEntityTypeConfiguration<Series>
    {
        public void Configure(EntityTypeBuilder<Series> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.UserId, s.Name })
                .IsUnique();

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(s => s.Description)
                   .HasMaxLength(250);

            builder.Property(s => s.PhotoUrl)
                   .HasMaxLength(2048);

            builder.Property(s => s.FileName)
                   .HasMaxLength(250);

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.Property(s => s.UpdatedAt)
                   .IsRequired();

            builder.HasOne(s => s.User)
                   .WithMany(u => u.Series)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Journeys)
                   .WithOne(j => j.Series)
                   .HasForeignKey(j => j.SeriesId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
