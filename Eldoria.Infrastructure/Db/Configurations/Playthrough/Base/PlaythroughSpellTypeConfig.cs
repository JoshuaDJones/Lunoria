using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughSpellTypeConfig : IEntityTypeConfiguration<PlaythroughSpellType>
{
    public void Configure(EntityTypeBuilder<PlaythroughSpellType> builder)
    {
        builder.ToTable("PlaythroughSpellTypes");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceSpellTypeId }).IsUnique();

        builder.Property(x => x.TypeName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.PhotoUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.SpellTypes)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
