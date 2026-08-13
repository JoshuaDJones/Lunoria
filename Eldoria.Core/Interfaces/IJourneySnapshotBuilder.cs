using Eldoria.Core.Snapshots;

namespace Eldoria.Core.Interfaces;

public interface IJourneySnapshotBuilder
{
    Task<JourneySnapshotV1?> BuildAsync(int userId, int journeyId, CancellationToken ct);
}
