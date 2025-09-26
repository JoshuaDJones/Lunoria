using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyCharacterItemConfig : IEntityTypeConfiguration<JourneyCharacterItem>
    {
        public void Configure(EntityTypeBuilder<JourneyCharacterItem> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.IsUsed)
                .IsRequired();

            builder.HasOne(c => c.Item)
                   .WithMany(i => i.JourneyCharacterItems)
                   .HasForeignKey(i => i.JourneyCharacterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
