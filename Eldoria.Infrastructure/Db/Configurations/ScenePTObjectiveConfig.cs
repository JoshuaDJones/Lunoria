using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public class ScenePTObjectiveConfig : IEntityTypeConfiguration<ScenePTObjective>
{
    public void Configure(EntityTypeBuilder<ScenePTObjective> b)
    {
        b.ToTable("ScenePTObjectives");
        b.HasKey(x => x.Id);
        b.Property(x => x.SortOrder).HasDefaultValue(0);
        b.HasOne(x => x.Scene).WithMany(x => x.Objectives).HasForeignKey(x => x.ScenePTId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.ScenePTId, x.SortOrder });
    }
}
