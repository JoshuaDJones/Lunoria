using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class ConsumableItemConfig : IEntityTypeConfiguration<ConsumableItem>
    {
        public void Configure(EntityTypeBuilder<ConsumableItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasIndex(i => new { i.UserId, i.Name })
                .IsUnique();

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

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.UpdatedAt)
                .IsRequired();
        }
    }
}
