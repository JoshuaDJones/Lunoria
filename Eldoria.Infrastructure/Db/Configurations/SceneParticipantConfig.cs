using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneParticipantConfig : IEntityTypeConfiguration<SceneParticipant>
    {
        public void Configure(EntityTypeBuilder<SceneParticipant> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasAlternateKey(p => new { p.Id, p.SceneProgressId });

            builder.ToTable(p => p.HasCheckConstraint(
                "CK_SceneParticipants_Character",
                "([JourneyCharacterId] IS NOT NULL AND [SceneCharacterId] IS NULL) OR " +
                "([JourneyCharacterId] IS NULL AND [SceneCharacterId] IS NOT NULL)"));

            builder.HasIndex(p => new { p.SceneProgressId, p.JourneyCharacterId })
                   .IsUnique()
                   .HasFilter("[JourneyCharacterId] IS NOT NULL");

            builder.HasIndex(p => new { p.SceneProgressId, p.SceneCharacterId })
                   .IsUnique()
                   .HasFilter("[SceneCharacterId] IS NOT NULL");

            builder.HasOne(p => p.SceneProgress)
                   .WithMany(sp => sp.Participants)
                   .HasForeignKey(p => p.SceneProgressId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.JourneyCharacter)
                   .WithMany(jc => jc.SceneParticipants)
                   .HasForeignKey(p => p.JourneyCharacterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SceneCharacter)
                   .WithMany(sc => sc.SceneParticipants)
                   .HasForeignKey(p => p.SceneCharacterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
