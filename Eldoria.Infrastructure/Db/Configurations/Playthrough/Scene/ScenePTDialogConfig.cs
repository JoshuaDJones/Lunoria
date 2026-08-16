using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTDialogConfig : IEntityTypeConfiguration<ScenePTDialog>
{
    public void Configure(EntityTypeBuilder<ScenePTDialog> builder)
    {
        builder.ToTable("ScenePTDialogs");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePTId, x.SourceSceneDialogId }).IsUnique();
        builder.Property(x => x.Title).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.ScenePT)
            .WithMany(x => x.SceneDialogs)
            .HasForeignKey(x => x.ScenePTId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
