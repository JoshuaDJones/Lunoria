using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
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
        builder.Property(x => x.PageType)
            .IsRequired()
            .HasDefaultValue(DialogPageType.Image);
        builder.Property(x => x.MediaUrl)
            .IsRequired()
            .HasMaxLength(2048);
        builder.Property(x => x.MediaBlobName)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.MediaContentType)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(x => x.SceneDialog)
            .WithMany(x => x.DialogPages)
            .HasForeignKey(x => x.SceneDialogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
