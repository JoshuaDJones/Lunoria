using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyIntroPageConfig : IEntityTypeConfiguration<JourneyIntroPage>
    {
        public void Configure(EntityTypeBuilder<JourneyIntroPage> builder)
        {
            builder.HasKey(ip => ip.Id);

            builder.Property(ip => ip.Type)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(ip => ip.Config)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(ip => ip.SortOrder)
                .IsRequired();

            builder.Property(ip => ip.PreviewPhotoUrl)
                .HasMaxLength(2048)
                .IsRequired(false);

            builder.HasOne(ip => ip.Journey)
                .WithMany(j => j.IntroPages)
                .HasForeignKey(ip => ip.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ip => new { ip.JourneyId, ip.SortOrder })
                .IsUnique();
        }
    }
}
