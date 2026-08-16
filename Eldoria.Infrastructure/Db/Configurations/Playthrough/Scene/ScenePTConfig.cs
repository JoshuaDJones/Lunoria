using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTConfig : IEntityTypeConfiguration<ScenePT>
{
    public void Configure(EntityTypeBuilder<ScenePT> builder)
    {
        builder.ToTable("ScenePTs");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceSceneId }).IsUnique();
        builder.HasIndex(x => new { x.PlaythroughId, x.SortOrder }).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).HasMaxLength(250);
        builder.Property(x => x.PhotoUrl).HasMaxLength(2048);
        builder.Property(x => x.FileName).HasMaxLength(250);
        builder.Property(x => x.GridUrl).HasMaxLength(2048);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.Scenes)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CurrentParticipant)
            .WithOne()
            .HasForeignKey<ScenePT>(x => x.CurrentParticipantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
