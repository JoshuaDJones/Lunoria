using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class DialogPageConfig : IEntityTypeConfiguration<DialogPage>
    {
        public void Configure(EntityTypeBuilder<DialogPage> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderNum)
                .IsRequired();

            builder.Property(p => p.CreateDate)
                .IsRequired();

            builder.Property(p => p.UpdateDate)
                .IsRequired();

            builder.HasMany(p => p.DialogPageSections)
                .WithOne(d => d.DialogPage)
                .HasForeignKey(p => p.DialogPageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
