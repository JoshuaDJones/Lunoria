using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Application.Common
{
    public static class SceneDialogMappings
    {
        public static SceneDialogDto ToDto(this SceneDialog sceneDialog)
        {
            return new SceneDialogDto
            {
                Id = sceneDialog.Id,
                Title = sceneDialog.Title,
                DialogPages = sceneDialog.DialogPages.Select(p => p.ToDto()).ToList()
            };
        }
    }
}
