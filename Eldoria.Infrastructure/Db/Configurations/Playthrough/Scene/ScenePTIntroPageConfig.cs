using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTIntroPageConfig : IEntityTypeConfiguration<ScenePTIntroPage>
{
    public void Configure(EntityTypeBuilder<ScenePTIntroPage> builder)
    {
        builder.ToTable("ScenePTIntroPages");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePTId, x.SourceIntroPageId }).IsUnique();
        builder.HasIndex(x => new { x.ScenePTId, x.SortOrder }).IsUnique();

        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Config).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.PreviewPhotoUrl).HasMaxLength(2048);

        builder.HasOne(x => x.ScenePT)
            .WithMany(x => x.IntroPages)
            .HasForeignKey(x => x.ScenePTId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
