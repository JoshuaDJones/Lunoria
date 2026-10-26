using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SceneDialogMappings
    {
        public static SceneDialogDto ToDto(this SceneDialog sceneDialog, int userId)
        {
            return new SceneDialogDto
            {
                Id = sceneDialog.Id,
                Title = sceneDialog.Title,
                DialogPages = sceneDialog.DialogPages.Select(p => p.ToDto(userId)).ToList()
            };
        }
    }
}
