using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneIntroPageMappings
{
    public static SceneIntroPageDto ToDto(this SceneIntroPage introPage) => new()
    {
        Id = introPage.Id,
        SortOrder = introPage.SortOrder,
        Type = introPage.Type,
        Config = introPage.Config,
        PreviewPhotoUrl = introPage.PreviewPhotoUrl,
        Scene = introPage.Scene.ToDto()
    };
}
