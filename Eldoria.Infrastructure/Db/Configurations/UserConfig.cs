using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(250);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.IsLocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();

            builder.HasQueryFilter(u => !u.IsDeleted);

            builder.HasMany(u => u.Characters)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Series)
                   .WithOne(s => s.User)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Journeys)
                   .WithOne(j => j.User)
                   .HasForeignKey(j => j.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Items)
                   .WithOne(i => i.User)
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.EquippableItems)
                   .WithOne(i => i.User)
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Spells)
                   .WithOne(s => s.User)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.SpellTypes)
                   .WithOne(st => st.User)
                   .HasForeignKey(st => st.UserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
