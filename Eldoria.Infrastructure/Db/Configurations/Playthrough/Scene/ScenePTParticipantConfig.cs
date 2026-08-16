using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTParticipantConfig : IEntityTypeConfiguration<ScenePTParticipant>
{
    public void Configure(EntityTypeBuilder<ScenePTParticipant> builder)
    {
        builder.ToTable("ScenePTParticipants", table =>
        {
            table.HasCheckConstraint(
                "CK_ScenePTParticipants_Character",
                "([JourneyPlaythroughCharacterId] IS NOT NULL AND [ScenePlaythroughCharacterId] IS NULL) OR " +
                "([JourneyPlaythroughCharacterId] IS NULL AND [ScenePlaythroughCharacterId] IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePlaythroughId, x.JourneyPlaythroughCharacterId })
            .IsUnique()
            .HasFilter("[JourneyPlaythroughCharacterId] IS NOT NULL");
        builder.HasIndex(x => new { x.ScenePlaythroughId, x.ParticipantType, x.SortOrderWithinType });

        builder.HasOne(x => x.ScenePlaythrough)
            .WithMany(x => x.SceneParticipants)
            .HasForeignKey(x => x.ScenePlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.JourneyPlaythroughCharacter)
            .WithMany(x => x.SceneParticipants)
            .HasForeignKey(x => x.JourneyPlaythroughCharacterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ScenePlaythroughCharacter)
            .WithOne(x => x.SceneParticipant)
            .HasForeignKey<ScenePTParticipant>(x => x.ScenePlaythroughCharacterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
