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

            builder.Property(sd => sd.OrderNum)
                .IsRequired();

            builder.Property(sd => sd.PhotoUrl)
                .HasMaxLength(2048);

            builder.Property(sd => sd.FileName)
                .HasMaxLength(250);

            builder.Property(sd => sd.Dialog)
                    .IsRequired()
                    .HasMaxLength(500);

            builder.Property(sd => sd.CreateDate)
                    .IsRequired();

            builder.Property(sd => sd.UpdateDate)
                    .IsRequired();

            builder.HasOne(sd => sd.Character)
                   .WithMany()
                   .HasForeignKey(sd => sd.CharacterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
