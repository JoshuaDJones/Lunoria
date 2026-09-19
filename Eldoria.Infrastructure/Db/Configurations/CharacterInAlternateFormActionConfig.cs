using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class CharacterInAlternateFormActionConfig : IEntityTypeConfiguration<CharacterInAlternateFormAction>
{
    public void Configure(EntityTypeBuilder<CharacterInAlternateFormAction> builder)
    {
        builder.ToTable("CharacterInAlternateFormActions");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.SceneEventAction)
            .WithOne(x => x.CharacterInAlternateFormAction)
            .HasForeignKey<CharacterInAlternateFormAction>(x => x.SceneEventActionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
