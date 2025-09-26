using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ItemConfig : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(i => i.PhotoUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(i => i.FileName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(i => i.HpEffect)
                .IsRequired();

            builder.Property(i => i.MpEffect)
                .IsRequired();

            builder.Property(i => i.CreateDate)
                .IsRequired();

            builder.Property(i => i.UpdateDate)
                .IsRequired();

            builder.HasMany(i => i.SceneCharacterItems)
                   .WithOne(ci => ci.Item)
                   .HasForeignKey(ci => ci.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.JourneyCharacterItems)
                   .WithOne(ci => ci.Item)
                   .HasForeignKey(ci => ci.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
