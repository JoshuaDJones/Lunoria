using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneParticipantTurnConfig : IEntityTypeConfiguration<SceneParticipantTurn>
    {
        public void Configure(EntityTypeBuilder<SceneParticipantTurn> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TurnPosition)
                   .IsRequired();

            builder.HasIndex(t => new { t.SceneProgressId, t.TurnPosition })
                   .IsUnique();

            builder.HasOne(t => t.SceneProgress)
                   .WithMany(p => p.ParticipantTurns)
                   .HasForeignKey(t => t.SceneProgressId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.SceneParticipant)
                   .WithMany(p => p.Turns)
                   .HasForeignKey(t => new { t.SceneParticipantId, t.SceneProgressId })
                   .HasPrincipalKey(p => new { p.Id, p.SceneProgressId })
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
