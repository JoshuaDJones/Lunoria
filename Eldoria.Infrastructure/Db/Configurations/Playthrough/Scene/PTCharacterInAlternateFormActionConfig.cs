using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene
{
    public class PTCharacterInAlternateFormActionConfig : IEntityTypeConfiguration<PTCharacterInAlternateFormAction>
    {
        public void Configure(EntityTypeBuilder<PTCharacterInAlternateFormAction> builder)
        {
            builder.ToTable("PTCharacterInAlternateFormActions");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ScenePTActionEvent)
                .WithOne(x => x.CharacterInAlternateFormAction)
                .HasForeignKey<PTCharacterInAlternateFormAction>(x => x.ScenePTActionEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PlaythroughCharacter)
                .WithMany()
                .HasForeignKey(x => x.PlaythroughCharacterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
