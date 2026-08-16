using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughConfig : IEntityTypeConfiguration<Eldoria.Core.Entities.Playthrough.Base.Playthrough>
{
    public void Configure(EntityTypeBuilder<Eldoria.Core.Entities.Playthrough.Base.Playthrough> builder)
    {
        builder.ToTable("Playthroughs");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.SourceJourneyId })
            .IsUnique()
            .HasFilter("[CompletedAt] IS NULL");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(250);
        builder.Property(x => x.PhotoUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.StartedAt).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Playthroughs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
