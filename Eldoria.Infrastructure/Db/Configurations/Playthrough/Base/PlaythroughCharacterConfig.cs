using Eldoria.Core.Entities.Playthrough.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Base;

public sealed class PlaythroughCharacterConfig : IEntityTypeConfiguration<PlaythroughCharacter>
{
    public void Configure(EntityTypeBuilder<PlaythroughCharacter> builder)
    {
        builder.ToTable("PlaythroughCharacters");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceCharacterId }).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(250);
        builder.Property(x => x.PhotoUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.PortraitUrl).HasMaxLength(2048);
        builder.Property(x => x.PortraitFileName).HasMaxLength(250);
        builder.Property(x => x.DialogActiveColor).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DialogInActiveColor).IsRequired().HasMaxLength(50);

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.Characters)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BaseAlternateForm)
            .WithOne()
            .HasForeignKey<PlaythroughCharacter>(x => x.BaseAlternateFormId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
