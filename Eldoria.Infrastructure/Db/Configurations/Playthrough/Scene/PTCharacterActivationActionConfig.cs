using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations.Playthrough.Scene
{
    public class PTCharacterActivationActionConfig : IEntityTypeConfiguration<PTCharacterActivationAction>
    {
        public void Configure(EntityTypeBuilder<PTCharacterActivationAction> builder)
        {
            builder.ToTable("PTCharacterActivationActions");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ScenePTActionEvent)
                .WithOne(x => x.CharacterActivationAction)
                .HasForeignKey<PTCharacterActivationAction>(x => x.ScenePTActionEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PlaythroughCharacter)
                .WithMany()
                .HasForeignKey(x => x.PlaythroughCharacterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
