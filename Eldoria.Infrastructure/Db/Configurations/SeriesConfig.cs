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

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(s => s.Description)
                   .HasMaxLength(250);

            builder.Property(s => s.PhotoUrl)
                   .HasMaxLength(2048);

            builder.Property(s => s.FileName)
                   .HasMaxLength(250);

            builder.Property(s => s.CreateDate)
                   .IsRequired();

            builder.Property(s => s.UpdateDate)
                   .IsRequired();

            builder.HasOne(s => s.User)
                   .WithMany(u => u.Series)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Journeys)
                   .WithOne(j => j.Series)
                   .HasForeignKey(j => j.SeriesId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
