using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class CharacterDialogSettingsConfig : IEntityTypeConfiguration<CharacterDialogSettings>
    {
        public void Configure(EntityTypeBuilder<CharacterDialogSettings> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DialogActiveColor)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.DialogUnActiveColor)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasQueryFilter(d => !d.Character.IsDeleted);
        }
    }
}
