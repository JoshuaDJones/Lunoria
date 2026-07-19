using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class DialogPageConfig : IEntityTypeConfiguration<DialogPage>
    {
        public void Configure(EntityTypeBuilder<DialogPage> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => new { p.SceneDialogId, p.OrderNum})
                .IsUnique();

            builder.Property(p => p.OrderNum)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.Property(p => p.PhotoUrl)
                .IsRequired(false)
                .HasMaxLength(2048);

            builder.Property(p => p.FileName)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.HasMany(p => p.DialogPageSections)
                .WithOne(d => d.DialogPage)
                .HasForeignKey(p => p.DialogPageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
