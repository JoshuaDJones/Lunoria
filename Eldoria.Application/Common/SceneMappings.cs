using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SceneMappings
    {
        public static SceneDto ToDto(this Scene scene, int userId)
        {
            return new SceneDto 
            {  
                Id = scene.Id,
                JourneyId = scene.JourneyId,
                Name = scene.Name,
                Description = scene.Description,
                PhotoUrl = scene.PhotoUrl,
                GridUrl = scene.GridUrl,
                SortOrder = scene.SortOrder,
                CreatedAt = scene.CreatedAt,
                Grid = scene.Grid?.ToDto(),
                SceneDialogs = scene.SceneDialogs.Select(s => s.ToDto(userId)).ToList(),
                SceneCharacters = scene.SceneCharacters.Select(sc => sc.ToDto()).ToList()
            };
        }
    }
}
