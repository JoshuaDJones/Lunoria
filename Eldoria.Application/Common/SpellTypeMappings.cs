using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SpellTypeMappings
    {
        public static SpellTypeDto ToDto(this SpellType spellType)
        {
            return new SpellTypeDto
            {
                Id = spellType.Id,
                Name = spellType.TypeName,
                Description = spellType.Description,
                PhotoUrl = spellType.PhotoUrl,
            };
        }
    }
}
