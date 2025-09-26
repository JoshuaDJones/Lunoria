using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class CharacterSpellConfig : IEntityTypeConfiguration<CharacterSpell>
    {
        public void Configure(EntityTypeBuilder<CharacterSpell> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Spell)
                   .WithMany(s => s.CharacterSpells)
                   .HasForeignKey(s => s.SpellId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
