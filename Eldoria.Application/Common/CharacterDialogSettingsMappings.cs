using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class CharacterDialogSettingsMappings
    {
        public static CharacterDialogSettingsDto ToDto(this CharacterDialogSettings characterDialogSettings)
        {
            return new CharacterDialogSettingsDto
            {
                Id = characterDialogSettings.Id,
                DialogActiveColor = characterDialogSettings.DialogActiveColor,
                DialogUnActiveColor = characterDialogSettings.DialogUnActiveColor
            };
        }
    }
}
