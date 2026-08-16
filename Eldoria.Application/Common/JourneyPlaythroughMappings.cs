using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Snapshots;
using System.Text.Json;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughMappings
    {
        public static JourneyPlaythroughDto ToDto(this Playthrough playthrough)
        {
            return new JourneyPlaythroughDto
            {
                Id = playthrough.Id,
                JourneyId = playthrough.SourceJourneyId,
                RevisionId = playthrough.JourneyRevisionId,
                RevisionNumber = playthrough.JourneyRevision.RevisionNumber,
                SnapshotSchemaVersion = playthrough.JourneyRevision.SchemaVersion,
                Snapshot = JsonSerializer.Deserialize<JourneySnapshotV1>(
                    playthrough.JourneyRevision.SnapshotJson,
                    SnapshotJsonOptions) ?? throw new InvalidOperationException(
                        $"Journey revision {playthrough.JourneyRevisionId} contains an invalid snapshot."),
                StartedAt = playthrough.StartedAt,
                CompletedAt = playthrough.CompletedAt,
                IsActive = playthrough.IsActive,
            };
        }

        private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
