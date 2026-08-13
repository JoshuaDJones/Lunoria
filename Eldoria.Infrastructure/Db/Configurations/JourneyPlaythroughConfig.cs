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

            builder.Property(p => p.SourceJourneyId).IsRequired();

            builder.HasIndex(p => p.SourceJourneyId)
                   .IsUnique()
                   .HasFilter("[IsActive] = 1");

            builder.HasOne(p => p.Journey)
                .WithMany(j => j.Playthroughs)
                .HasForeignKey(p => p.JourneyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.JourneyRevision)
                .WithMany(r => r.Playthroughs)
                .HasForeignKey(p => p.JourneyRevisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.JourneyCharacters)
                .WithOne(c => c.JourneyPlaythrough)
                .HasForeignKey(c => c.JourneyPlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ScenePlaythroughs)
                .WithOne(s => s.JourneyPlaythrough)
                .HasForeignKey(s => s.JourneyPlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PlaythroughEventLogs)
                .WithOne(l => l.JourneyPlaythrough)
                .HasForeignKey(l => l.JourneyPlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
