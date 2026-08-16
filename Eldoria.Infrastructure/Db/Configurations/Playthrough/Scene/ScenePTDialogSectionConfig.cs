using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTDialogSectionConfig : IEntityTypeConfiguration<ScenePTDialogSection>
{
    public void Configure(EntityTypeBuilder<ScenePTDialogSection> builder)
    {
        builder.ToTable("ScenePTDialogSections");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.DialogPageId, x.SourceDialogSectionId }).IsUnique();
        builder.HasIndex(x => new { x.DialogPageId, x.OrderNum }).IsUnique();
        builder.Property(x => x.ReadingText).IsRequired().HasColumnType("nvarchar(max)");

        builder.HasOne(x => x.DialogPage)
            .WithMany(x => x.DialogPageSections)
            .HasForeignKey(x => x.DialogPageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Character)
            .WithMany(x => x.DialogSections)
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
