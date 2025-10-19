using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class DialogPageSectionConfig : IEntityTypeConfiguration<DialogPageSection>
    {
        public void Configure(EntityTypeBuilder<DialogPageSection> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.OrderNum)
                .IsRequired();

            builder.Property(s => s.ReadingText)
                .IsRequired();

            builder.Property(s => s.IsNarrator)
                .IsRequired();

            builder.Property(s => s.CreateDate)
                .IsRequired();

            builder.Property(s => s.UpdateDate)
                .IsRequired();

            builder.HasOne(s => s.Character)
                .WithMany()
                .HasForeignKey(s => s.CharacterId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
