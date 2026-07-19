using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations
{
    public class SceneCharacterSpellConfig : IEntityTypeConfiguration<SceneCharacterSpell>
    {
        public void Configure(EntityTypeBuilder<SceneCharacterSpell> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.SceneCharacterId, s.SpellId })
                .IsUnique();

            builder.HasOne(s => s.SceneCharacter)
                .WithMany(c => c.SceneCharacterSpells)
                .HasForeignKey(s => s.SceneCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Spell)
                .WithMany()
                .HasForeignKey(s => s.SpellId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(s => !s.SceneCharacter.Character.IsDeleted);
        }
    }
}
