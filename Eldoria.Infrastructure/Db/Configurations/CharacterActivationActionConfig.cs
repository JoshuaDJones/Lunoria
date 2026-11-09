using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class CharacterActivationActionConfig : IEntityTypeConfiguration<CharacterActivationAction>
{
    public void Configure(EntityTypeBuilder<CharacterActivationAction> builder)
    {
        builder.ToTable("CharacterActivationActions");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.SceneEventAction)
            .WithOne(x => x.CharacterActivationAction)
            .HasForeignKey<CharacterActivationAction>(x => x.SceneEventActionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
