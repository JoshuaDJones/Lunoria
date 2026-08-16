using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTEventConfig : IEntityTypeConfiguration<ScenePTEvent>
{
    public void Configure(EntityTypeBuilder<ScenePTEvent> builder)
    {
        builder.ToTable("ScenePTEvents");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePTId, x.SourceSceneEventId }).IsUnique();
        builder.HasIndex(x => new { x.ScenePTId, x.SortOrder }).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);

        builder.HasOne(x => x.ScenePT)
            .WithMany(x => x.SceneEvents)
            .HasForeignKey(x => x.ScenePTId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
