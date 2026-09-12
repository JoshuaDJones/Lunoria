using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class CharacterChangeAlternateFormActionConfig : IEntityTypeConfiguration<CharacterChangeAlternateFormAction>
{
    public void Configure(EntityTypeBuilder<CharacterChangeAlternateFormAction> builder)
    {
        builder.ToTable("CharacterChangeAlternateFormActions");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.SceneEventAction).WithOne(x => x.CharacterChangeAlternateFormAction)
            .HasForeignKey<CharacterChangeAlternateFormAction>(x => x.SceneEventActionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.AlternateForm).WithMany().HasForeignKey(x => x.AlternateFormId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
