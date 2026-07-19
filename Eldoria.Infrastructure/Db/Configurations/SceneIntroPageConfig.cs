using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneIntroPageConfig : IEntityTypeConfiguration<SceneIntroPage>
    {
        public void Configure(EntityTypeBuilder<SceneIntroPage> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => new { p.SceneId, p.SortOrder })
                .IsUnique();

            builder.Property(p => p.SortOrder).IsRequired();
            builder.Property(p => p.Type)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Config)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.PreviewPhotoUrl)
                .IsRequired(false)
                .HasMaxLength(2048);
        }
    }
}
