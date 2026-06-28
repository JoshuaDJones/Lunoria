using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyPlaythroughConfig : IEntityTypeConfiguration<JourneyPlaythrough>
    {
        public void Configure(EntityTypeBuilder<JourneyPlaythrough> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.StartedAt)
                   .IsRequired();

            builder.Property(p => p.CompletedAt)
                   .IsRequired(false);

            builder.Property(p => p.IsActive)
                   .IsRequired();

            builder.HasIndex(p => p.JourneyId)
                   .IsUnique()
                   .HasFilter("[IsActive] = 1");
        }
    }
}
