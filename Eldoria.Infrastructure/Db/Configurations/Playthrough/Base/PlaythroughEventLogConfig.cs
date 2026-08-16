using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughEventLogConfig : IEntityTypeConfiguration<PlaythroughEventLog>
{
    public void Configure(EntityTypeBuilder<PlaythroughEventLog> builder)
    {
        builder.ToTable("PlaythroughEventLogs");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.EventTime });
        builder.Property(x => x.Message).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.EventTime).IsRequired();

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.EventLogs)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
