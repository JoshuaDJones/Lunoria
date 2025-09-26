using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class JourneyConfig : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(j => j.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(j => j.PhotoUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(j => j.FileName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(j => j.CreateDate)
                .IsRequired();

            builder.Property(j => j.UpdateDate)
                .IsRequired();

            builder.HasMany(j => j.Scenes)
                   .WithOne(s => s.Journey)
                   .HasForeignKey(s => s.JourneyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.JourneyCharacters)
                   .WithOne(jc => jc.Journey)
                   .HasForeignKey(jc => jc.JourneyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
