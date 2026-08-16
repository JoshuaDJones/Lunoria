using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughIntroPageConfig : IEntityTypeConfiguration<PlaythroughIntroPage>
{
    public void Configure(EntityTypeBuilder<PlaythroughIntroPage> builder)
    {
        builder.ToTable("PlaythroughIntroPages");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceIntroPageId }).IsUnique();
        builder.HasIndex(x => new { x.PlaythroughId, x.SortOrder }).IsUnique();

        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Config).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.PreviewPhotoUrl).HasMaxLength(2048);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.IntroPages)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
