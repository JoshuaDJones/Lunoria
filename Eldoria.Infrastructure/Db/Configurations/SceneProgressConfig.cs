using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneProgressConfig : IEntityTypeConfiguration<SceneProgress>
    {
        public void Configure(EntityTypeBuilder<SceneProgress> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.SceneProgressStatus)
                   .IsRequired();

            builder.HasIndex(p => new { p.JourneyPlaythroughId, p.SceneId })
                   .IsUnique();

            builder.HasOne(p => p.JourneyPlaythrough)
                   .WithMany(jp => jp.SceneProgressRecords)
                   .HasForeignKey(p => p.JourneyPlaythroughId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
