using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneDialogConfig : IEntityTypeConfiguration<SceneDialog>
    {
        public void Configure(EntityTypeBuilder<SceneDialog> builder)
        {
            builder.HasKey(sd => sd.Id);

            builder.Property(sd => sd.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(sd => sd.CreatedAt)
                    .IsRequired();

            builder.Property(sd => sd.UpdatedAt)
                    .IsRequired();

            builder.HasMany(sd => sd.DialogPages)
                   .WithOne(dp => dp.SceneDialog)
                   .HasForeignKey(sd => sd.SceneDialogId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
