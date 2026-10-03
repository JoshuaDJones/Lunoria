using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public class ScenePTObjectiveSelectionConfig : IEntityTypeConfiguration<ScenePT>
{
    public void Configure(EntityTypeBuilder<ScenePT> b) =>
        b.Property(x => x.CurrentObjectiveIndex).HasDefaultValue(0);
}
