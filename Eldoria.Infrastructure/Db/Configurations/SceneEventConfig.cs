using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneEventConfig : IEntityTypeConfiguration<SceneEvent>
    {
        public void Configure(EntityTypeBuilder<SceneEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.SceneId, e.SortOrder })
                .IsUnique();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(e => e.Description)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(e => e.SortOrder).IsRequired();

            builder.HasMany(e => e.SceneEventActions)
                .WithOne(a => a.SceneEvent)
                .HasForeignKey(a => a.SceneEventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
