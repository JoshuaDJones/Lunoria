using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class CharacterStatAdjustmentActionConfig : IEntityTypeConfiguration<CharacterStatAdjustmentAction>
    {
        public void Configure(EntityTypeBuilder<CharacterStatAdjustmentAction> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CharacterStatType)
                .IsRequired();

            builder.Property(c => c.AdjustmentOperation)
                .IsRequired();

            builder.Property(c => c.Value)
                .IsRequired();

            builder.HasOne(c => c.Character)
                .WithMany()
                .HasForeignKey(c => c.CharacterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.SceneEventAction)
                .WithOne(c => c.CharacterStatAdjustmentAction)
                .HasForeignKey<CharacterStatAdjustmentAction>(c => c.SceneEventActionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}