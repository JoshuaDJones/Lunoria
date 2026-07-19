using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SeriesMappings
{
    public static SeriesDto ToDto(this Series series) => new()
    {
        Id = series.Id,
        Name = series.Name,
        Description = series.Description,
        PhotoUrl = series.PhotoUrl,
        FileName = series.FileName,
        CreatedAt = series.CreatedAt,
        UpdatedAt = series.UpdatedAt,
        Journeys = [.. series.Journeys.Select(journey => journey.ToDto())]
    };
}
