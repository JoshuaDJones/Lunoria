using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTDialogPageConfig : IEntityTypeConfiguration<ScenePTDialogPage>
{
    public void Configure(EntityTypeBuilder<ScenePTDialogPage> builder)
    {
        builder.ToTable("ScenePTDialogPages");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.SceneDialogId, x.SourceDialogPageId }).IsUnique();
        builder.HasIndex(x => new { x.SceneDialogId, x.OrderNum }).IsUnique();
        builder.Property(x => x.PhotoUrl).HasMaxLength(2048);
        builder.Property(x => x.FileName).HasMaxLength(255);

        builder.HasOne(x => x.SceneDialog)
            .WithMany(x => x.DialogPages)
            .HasForeignKey(x => x.SceneDialogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
