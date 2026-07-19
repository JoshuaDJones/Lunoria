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

            builder.HasIndex(s => new { s.JourneyId, s.SortOrder })
                .IsUnique();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.Description)
                .IsRequired(false)
                .HasMaxLength(250);

            builder.Property(s => s.PhotoUrl)
                .IsRequired(false)
                .HasMaxLength(2048);

            builder.Property(s => s.FileName)
                .IsRequired(false)
                .HasMaxLength(250);

            builder.Property(s => s.GridUrl)
                .IsRequired(false)
                .HasMaxLength(2048);

            builder.Property(s => s.SortOrder)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .IsRequired();

            builder.HasMany(s => s.SceneDialogs)
                   .WithOne(sd => sd.Scene)
                   .HasForeignKey(s => s.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.SceneCharacters)
                   .WithOne(sc => sc.Scene)
                   .HasForeignKey(s => s.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.SceneChests)
                   .WithOne(c => c.Scene)
                   .HasForeignKey(c => c.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.SceneIntroPages)
                   .WithOne(p => p.Scene)
                   .HasForeignKey(p => p.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.SceneEvents)
                   .WithOne(e => e.Scene)
                   .HasForeignKey(e => e.SceneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ScenePlaythroughs)
                   .WithOne(p => p.Scene)
                   .HasForeignKey(p => p.SceneId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
