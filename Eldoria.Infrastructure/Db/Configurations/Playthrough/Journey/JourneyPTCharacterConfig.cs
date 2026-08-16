using Eldoria.Core.Entities.Playthrough.Journey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Journey;

public sealed class JourneyPTCharacterConfig : IEntityTypeConfiguration<JourneyPTCharacter>
{
    public void Configure(EntityTypeBuilder<JourneyPTCharacter> builder)
    {
        builder.ToTable("JourneyPTCharacters");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PlaythroughId, x.SourceJourneyCharacterId }).IsUnique();
        builder.HasIndex(x => new { x.PlaythroughId, x.PlaythroughCharacterId }).IsUnique();

        builder.HasOne(x => x.Playthrough)
            .WithMany(x => x.JourneyCharacters)
            .HasForeignKey(x => x.PlaythroughId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlaythroughCharacter)
            .WithMany()
            .HasForeignKey(x => x.PlaythroughCharacterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.AlternateForm)
            .WithMany()
            .HasForeignKey(x => x.AlternateFormId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
