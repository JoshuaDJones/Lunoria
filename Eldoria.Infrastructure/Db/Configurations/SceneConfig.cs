using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneConfig : IEntityTypeConfiguration<Scene>
    {
        public void Configure(EntityTypeBuilder<Scene> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.PhotoUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(s => s.FileName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.GridUrl)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.CreateDate)
                .IsRequired();

            builder.Property(s => s.UpdateDate)
                .IsRequired();

            builder.HasMany(s => s.SceneDialogs)
                   .WithOne(sd => sd.Scene)
                   .HasForeignKey(s => s.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.SceneCharacters)
                   .WithOne(sc => sc.Scene)
                   .HasForeignKey(s => s.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
