using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTActionEventConfig : IEntityTypeConfiguration<ScenePTActionEvent>
{
    public void Configure(EntityTypeBuilder<ScenePTActionEvent> builder)
    {
        builder.ToTable("ScenePTActionEvents");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePTEventId, x.SourceSceneEventActionId }).IsUnique();
        builder.HasIndex(x => new { x.ScenePTEventId, x.SortOrder }).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.SceneEvent)
            .WithMany(x => x.ScenePTActionEvents)
            .HasForeignKey(x => x.ScenePTEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
