using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class DialogPageConfig : IEntityTypeConfiguration<ScenePTDialogPage>
    {
        public void Configure(EntityTypeBuilder<ScenePTDialogPage> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => new { p.SceneDialogId, p.OrderNum})
                .IsUnique();

            builder.Property(p => p.OrderNum)
                .IsRequired();

            builder.Property(p => p.PageType)
                .IsRequired()
                .HasDefaultValue(DialogPageType.Image);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.Property(p => p.MediaUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(p => p.MediaBlobName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.MediaContentType)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasMany(p => p.DialogPageSections)
                .WithOne(d => d.DialogPage)
                .HasForeignKey(p => p.DialogPageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
