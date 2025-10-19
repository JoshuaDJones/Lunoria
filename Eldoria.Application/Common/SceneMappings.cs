using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SceneMappings
    {
        public static SceneDto ToDto(this Scene scene)
        {
            return new SceneDto 
            {  
                Id = scene.Id,
                JourneyId = scene.JourneyId,
                Name = scene.Name,
                Description = scene.Description,
                PhotoUrl = scene.PhotoUrl,
                GridUrl = scene.GridUrl,
                CreateDate = scene.CreateDate,
                SceneDialogs = scene.SceneDialogs.Select(s => s.ToDto()).ToList(),
                SceneCharacters = scene.SceneCharacters.Select(sc => sc.ToDto()).ToList()
            };
        }
    }
}
