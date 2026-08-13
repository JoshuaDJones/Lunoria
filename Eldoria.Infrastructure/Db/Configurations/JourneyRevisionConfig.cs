using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class JourneyRevisionConfig : IEntityTypeConfiguration<JourneyRevision>
{
    public void Configure(EntityTypeBuilder<JourneyRevision> builder)
    {
        builder.HasKey(revision => revision.Id);
        builder.Property(revision => revision.RevisionNumber).IsRequired();
        builder.Property(revision => revision.SchemaVersion).IsRequired();
        builder.Property(revision => revision.ContentHash).IsRequired().HasMaxLength(64);
        builder.Property(revision => revision.SnapshotJson).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(revision => revision.CreatedAt).IsRequired();

        builder.HasIndex(revision => new
            { revision.CreatedByUserId, revision.SourceJourneyId, revision.ContentHash })
            .IsUnique()
            .HasFilter("[SourceJourneyId] IS NOT NULL");
        builder.HasIndex(revision => new
            { revision.CreatedByUserId, revision.SourceJourneyId, revision.RevisionNumber })
            .IsUnique()
            .HasFilter("[SourceJourneyId] IS NOT NULL");

        builder.HasOne(revision => revision.SourceJourney)
            .WithMany(journey => journey.Revisions)
            .HasForeignKey(revision => revision.SourceJourneyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(revision => revision.CreatedByUser)
            .WithMany(user => user.JourneyRevisions)
            .HasForeignKey(revision => revision.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(revision => revision.Playthroughs)
            .WithOne(playthrough => playthrough.JourneyRevision)
            .HasForeignKey(playthrough => playthrough.JourneyRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
