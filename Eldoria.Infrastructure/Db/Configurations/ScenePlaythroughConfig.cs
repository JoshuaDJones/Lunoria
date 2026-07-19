using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughConfig : IEntityTypeConfiguration<ScenePlaythrough>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythrough> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Status)
                   .IsRequired();

            builder.Property(p => p.StartedAt)
                .IsRequired(false);

            builder.Property(p => p.EndedAt)
                .IsRequired(false);

            builder.Property(p => p.RoundNumber)
                .IsRequired();

            builder.HasIndex(p => new { p.JourneyPlaythroughId, p.SceneId })
                   .IsUnique();

            builder.HasOne(p => p.JourneyPlaythrough)
                   .WithMany(jp => jp.ScenePlaythroughs)
                   .HasForeignKey(p => p.JourneyPlaythroughId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.CurrentParticipant)
                .WithMany()
                .HasForeignKey(p => p.CurrentParticipantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(p => p.SceneCharacters)
                .WithOne(c => c.ScenePlaythrough)
                .HasForeignKey(c => c.ScenePlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Participants)
                .WithOne(x => x.ScenePlaythrough)
                .HasForeignKey(x => x.ScenePlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PlaythroughEvents)
                .WithOne(e => e.ScenePlaythrough)
                .HasForeignKey(e => e.ScenePlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PlaythroughChests)
                .WithOne(c => c.ScenePlaythrough)
                .HasForeignKey(c => c.ScenePlaythroughId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
