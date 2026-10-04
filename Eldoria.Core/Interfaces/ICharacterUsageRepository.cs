namespace Eldoria.Core.Interfaces;

public interface ICharacterUsageRepository
{
    Task<bool> HasDirectAssignmentsAsync(int characterId, CancellationToken ct);
}

