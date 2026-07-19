using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneEventActionConfig : IEntityTypeConfiguration<SceneEventAction>
    {
        public void Configure(EntityTypeBuilder<SceneEventAction> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasIndex(a => new { a.SceneEventId, a.SortOrder })
                .IsUnique();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(a => a.SortOrder).IsRequired();
            builder.Property(a => a.ActionTargetType).IsRequired();
            builder.Property(a => a.EventActionType).IsRequired();
        }
    }
}
