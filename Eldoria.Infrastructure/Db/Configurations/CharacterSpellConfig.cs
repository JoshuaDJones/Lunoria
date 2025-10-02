using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class CharacterSpellConfig : IEntityTypeConfiguration<CharacterSpell>
    {
        public void Configure(EntityTypeBuilder<CharacterSpell> builder)
        {
            builder.HasKey(cs => cs.Id);

            builder.HasOne(cs => cs.Character)
                   .WithMany(c => c.CharacterSpells)
                   .HasForeignKey(cs => cs.CharacterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Spell)
                   .WithMany(s => s.CharacterSpells)
                   .HasForeignKey(cs => cs.SpellId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
