using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ScenePlaythroughEventConfig : IEntityTypeConfiguration<ScenePlaythroughEvent>
    {
        public void Configure(EntityTypeBuilder<ScenePlaythroughEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.ScenePlaythroughId, e.SceneEventId })
                .IsUnique();

            builder.Property(e => e.ExecutionStatus).IsRequired();
            builder.Property(e => e.ErrorMessage)
                .IsRequired(false)
                .HasMaxLength(2000);
            builder.Property(e => e.StartedAt).IsRequired(false);
            builder.Property(e => e.CompletedAt).IsRequired(false);

            builder.HasOne(e => e.SceneEvent)
                .WithMany()
                .HasForeignKey(e => e.SceneEventId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
