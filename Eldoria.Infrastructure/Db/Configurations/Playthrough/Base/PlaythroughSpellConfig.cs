using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughSpellConfig : IEntityTypeConfiguration<PlaythroughSpell>
{
    public void Configure(EntityTypeBuilder<PlaythroughSpell> builder)
    {
        builder.ToTable("PlaythroughSpells", table =>
        {
            table.HasCheckConstraint("CK_PlaythroughSpells_Range", "[Range] >= 0");
            table.HasCheckConstraint("CK_PlaythroughSpells_MpCost", "[MpCost] >= 0");
        });
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceSpellId }).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(250);
        builder.Property(x => x.PhotoUrl).HasMaxLength(2048);
        builder.Property(x => x.FileName).HasMaxLength(250);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughSpellType)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.PlaythroughSpellTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
