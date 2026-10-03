using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public class SceneObjectivePointConfig : IEntityTypeConfiguration<SceneObjectivePoint>
{
    public void Configure(EntityTypeBuilder<SceneObjectivePoint> b)
    {
        b.ToTable("SceneObjectivePoints");
        b.HasKey(x => x.Id);
        b.Property(x => x.Text).HasMaxLength(1000).IsRequired();
        b.Property(x => x.SortOrder).HasDefaultValue(0);
        b.HasOne(x => x.SceneObjective).WithMany(x => x.Points).HasForeignKey(x => x.SceneObjectiveId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.SceneObjectiveId, x.SortOrder });
    }
}
