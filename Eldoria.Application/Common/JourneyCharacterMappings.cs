using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Application.Common
{
    public static class JourneyCharacterMappings
    {
        public static JourneyCharacterDto ToDto(this JourneyCharacter journeyCharacter)
        {
            return new JourneyCharacterDto
            {
                Id = journeyCharacter.Id,
                JourneyId = journeyCharacter.Id,
                CharacterId = journeyCharacter.CharacterId,
                CurrentHp = journeyCharacter.CurrentHp,
                CurrentMp = journeyCharacter.CurrentMp,
                IsDown = journeyCharacter.IsDown,
                IsAlternateForm = journeyCharacter.IsAlternateForm,
                Character = journeyCharacter.Character.ToDto(),
                JourneyCharacterItems = journeyCharacter.JourneyCharacterItems.Select(jci => jci.ToDto()).ToList()
            };
        }
    }
}
