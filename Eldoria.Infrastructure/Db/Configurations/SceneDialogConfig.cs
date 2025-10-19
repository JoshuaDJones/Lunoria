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
                .IsRequired();

            builder.Property(sd => sd.CreateDate)
                    .IsRequired();

            builder.Property(sd => sd.UpdateDate)
                    .IsRequired();

            builder.HasMany(sd => sd.DialogPages)
                   .WithOne(dp => dp.SceneDialog)
                   .HasForeignKey(sd => sd.SceneDialogId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
