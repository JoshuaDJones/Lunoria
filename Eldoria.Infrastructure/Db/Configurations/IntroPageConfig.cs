using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class IntroPageConfig: IEntityTypeConfiguration<IntroPage>
    {
        public void Configure(EntityTypeBuilder<IntroPage> builder)
        {
            builder.HasKey(ip => ip.Id);

            builder.Property(ip => ip.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(ip => ip.Config)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(ip => ip.Order)
                .IsRequired();

            builder.HasOne(ip => ip.Journey)
                .WithMany(j => j.IntroPages)
                .HasForeignKey(ip => ip.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ip => new { ip.JourneyId, ip.Order })
                .IsUnique();
        }
    }
}
