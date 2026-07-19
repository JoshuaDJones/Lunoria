using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughParticipantConfig : IEntityTypeConfiguration<ScenePlaythroughParticipant>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughParticipant> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.SortOrderWithinType).IsRequired(false);
            builder.Property(p => p.ParticipantType).IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_ScenePlaythroughParticipants_Character",
                    "([JourneyPlaythroughCharacterId] IS NOT NULL AND [ScenePlaythroughCharacterId] IS NULL) OR " +
                    "([JourneyPlaythroughCharacterId] IS NULL AND [ScenePlaythroughCharacterId] IS NOT NULL)");
                t.HasCheckConstraint(
                    "CK_ScenePlaythroughParticipants_ActiveOrder",
                    "([IsActive] = 1 AND [SortOrderWithinType] IS NOT NULL) OR " +
                    "([IsActive] = 0 AND [SortOrderWithinType] IS NULL)");
            });

            builder.HasIndex(p => new { p.ScenePlaythroughId, p.JourneyPlaythroughCharacterId })
                .IsUnique()
                .HasFilter("[JourneyPlaythroughCharacterId] IS NOT NULL");

            builder.HasIndex(p => new { p.ScenePlaythroughId, p.ScenePlaythroughCharacterId })
                .IsUnique()
                .HasFilter("[ScenePlaythroughCharacterId] IS NOT NULL");

            builder.HasIndex(p => new { p.ScenePlaythroughId, p.ParticipantType, p.SortOrderWithinType })
                .IsUnique()
                .HasFilter("[SortOrderWithinType] IS NOT NULL");

            builder.HasOne(p => p.JourneyPlaythroughCharacter)
                .WithMany(c => c.SceneParticipants)
                .HasForeignKey(p => p.JourneyPlaythroughCharacterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.ScenePlaythroughCharacter)
                .WithMany(c => c.SceneParticipants)
                .HasForeignKey(p => p.ScenePlaythroughCharacterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
