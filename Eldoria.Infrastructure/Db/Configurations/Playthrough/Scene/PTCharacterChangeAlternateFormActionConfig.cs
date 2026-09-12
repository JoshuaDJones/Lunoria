using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene;

public sealed class PTCharacterChangeAlternateFormActionConfig : IEntityTypeConfiguration<PTCharacterChangeAlternateFormAction>
{
    public void Configure(EntityTypeBuilder<PTCharacterChangeAlternateFormAction> builder)
    {
        builder.ToTable("PTCharacterChangeAlternateFormActions");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.ScenePTActionEvent).WithOne(x => x.CharacterChangeAlternateFormAction)
            .HasForeignKey<PTCharacterChangeAlternateFormAction>(x => x.ScenePTActionEventId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.PlaythroughCharacter).WithMany().HasForeignKey(x => x.PlaythroughCharacterId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.AlternateForm).WithMany().HasForeignKey(x => x.AlternateFormId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
