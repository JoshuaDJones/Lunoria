using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughJoinSessionConfig
    : IEntityTypeConfiguration<PlaythroughJoinSession>
{
    public void Configure(EntityTypeBuilder<PlaythroughJoinSession> builder)
    {
        builder.ToTable("PlaythroughJoinSessions");
        builder.HasKey(session => session.Id);

        builder.HasIndex(session => session.TokenHash).IsUnique();
        builder.HasIndex(session => session.PlaythroughId).IsUnique();
        builder.Property(session => session.TokenHash)
            .IsRequired()
            .HasMaxLength(64)
            .IsFixedLength();

        builder.HasOne(session => session.Playthrough)
            .WithOne(playthrough => playthrough.JoinSession)
            .HasForeignKey<PlaythroughJoinSession>(session => session.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
