using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneCharacterItemConfig : IEntityTypeConfiguration<SceneCharacterItem>
    {
        public void Configure(EntityTypeBuilder<SceneCharacterItem> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.IsUsed)
                .IsRequired();

            builder.HasOne(c => c.Item)
                   .WithMany(i => i.SceneCharacterItems)
                   .HasForeignKey(i => i.SceneCharacterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
