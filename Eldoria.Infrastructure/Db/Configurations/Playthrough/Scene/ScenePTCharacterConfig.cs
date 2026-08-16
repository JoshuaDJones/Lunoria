using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class ScenePTCharacterConfig : IEntityTypeConfiguration<ScenePTCharacter>
{
    public void Configure(EntityTypeBuilder<ScenePTCharacter> builder)
    {
        builder.ToTable("ScenePTCharacters");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ScenePlaythroughId, x.SourceSceneCharacterId }).IsUnique();
        builder.HasIndex(x => new { x.ScenePlaythroughId, x.PlaythroughCharacterId });

        builder.HasOne(x => x.ScenePlaythrough)
            .WithMany(x => x.SceneCharacters)
            .HasForeignKey(x => x.ScenePlaythroughId)
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
