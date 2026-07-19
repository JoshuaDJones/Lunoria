using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyPlaythroughEventLogConfig : IEntityTypeConfiguration<JourneyPlaythroughEventLog>
    {
        public void Configure(EntityTypeBuilder<JourneyPlaythroughEventLog> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(l => l.EventTime).IsRequired();
            builder.HasIndex(l => new { l.JourneyPlaythroughId, l.EventTime });
        }
    }
}
