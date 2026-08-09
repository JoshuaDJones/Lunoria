using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SceneGridMappings
    {
        public static SceneGridDto ToDto(this SceneGrid grid) => new()
        {
            Id = grid.Id,
            SceneId = grid.SceneId,
            Rows = grid.Rows,
            Columns = grid.Columns,
            GridColor = grid.GridColor,
            BackgroundImageUrl = grid.BackgroundImageUrl,
            CreatedAt = grid.CreatedAt,
            UpdatedAt = grid.UpdatedAt,
        };
    }
}
