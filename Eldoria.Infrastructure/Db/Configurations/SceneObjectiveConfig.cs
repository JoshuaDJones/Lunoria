using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public class SceneObjectiveConfig : IEntityTypeConfiguration<SceneObjective>
{
    public void Configure(EntityTypeBuilder<SceneObjective> b)
    {
        b.ToTable("SceneObjectives");
        b.HasKey(x => x.Id);
        b.Property(x => x.SortOrder).HasDefaultValue(0);
        b.HasOne(x => x.Scene).WithMany(x => x.Objectives).HasForeignKey(x => x.SceneId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.SceneId, x.SortOrder });
    }
}
