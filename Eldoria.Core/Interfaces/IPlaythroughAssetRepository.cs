namespace Eldoria.Core.Interfaces;

public interface IPlaythroughAssetRepository
{
    Task<bool> IsReferencedByRevisionAsync(string assetReference, CancellationToken ct);
}
