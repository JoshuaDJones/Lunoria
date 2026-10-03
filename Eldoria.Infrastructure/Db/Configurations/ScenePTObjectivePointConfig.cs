using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public class ScenePTObjectivePointConfig : IEntityTypeConfiguration<ScenePTObjectivePoint>
{
    public void Configure(EntityTypeBuilder<ScenePTObjectivePoint> b)
    {
        b.ToTable("ScenePTObjectivePoints");
        b.HasKey(x => x.Id);
        b.Property(x => x.Text).HasMaxLength(1000).IsRequired();
        b.Property(x => x.SortOrder).HasDefaultValue(0);
        b.HasOne(x => x.SceneObjective).WithMany(x => x.Points).HasForeignKey(x => x.ScenePTObjectiveId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.ScenePTObjectiveId, x.SortOrder });
    }
}
