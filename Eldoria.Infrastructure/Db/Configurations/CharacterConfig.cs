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
    public class CharacterConfig : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.PhotoUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(c => c.FileName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.MaxHp)
                .IsRequired();

            builder.Property(c => c.MaxMp)
                .IsRequired();

            builder.Property(c => c.Movement)
                .IsRequired();

            builder.Property(c => c.MaxInventory)
                .IsRequired();

            builder.Property(c => c.IsPlayer)
                .IsRequired();

            builder.Property(c => c.IsNPC)
                .IsRequired();

            builder.Property(c => c.IsEnemy)
                .IsRequired();

            builder.Property(c => c.CreateDate)
                .IsRequired();

            builder.Property(c => c.UpdateDate)
                .IsRequired();

            builder.HasOne(c => c.AlternateForm)
                   .WithOne()
                   .HasForeignKey<Character>(c => c.AlternateFormId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.CharacterSpells)
                   .WithOne(s => s.Character)
                   .HasForeignKey(c => c.SpellId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
