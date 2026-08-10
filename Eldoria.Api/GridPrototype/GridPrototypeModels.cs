namespace Eldoria.Api.GridPrototype;

public sealed record GridPrototypeCharacterDto(int Id, string Name, string ImageUrl, int CharacterType);

public sealed record GridPrototypeTokenDto(
    string Id, int CharacterId, string Name, string ImageUrl, int Row, int Column);

public sealed record GridPrototypeSessionSnapshot(
    string Code,
    int Rows,
    int Columns,
    string GridColor,
    string? BackgroundImage,
    IReadOnlyList<GridPrototypeTokenDto> Tokens);

public sealed record CreateGridPrototypeSessionResult(
    string HostToken,
    GridPrototypeSessionSnapshot Session);

internal sealed record GridPrototypeTokenLock(string ConnectionId, DateTime ExpiresAt);

internal sealed class GridPrototypeSession
{
    public object SyncRoot { get; } = new();
    public required string Code { get; init; }
    public required string HostToken { get; init; }
    public int Rows { get; init; } = GridPrototypeSessionStore.DefaultRows;
    public int Columns { get; init; } = GridPrototypeSessionStore.DefaultColumns;
    public string GridColor { get; set; } = "#ffffff";
    public string? BackgroundImage { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Dictionary<string, GridPrototypeTokenDto> Tokens { get; } = [];
    public Dictionary<string, GridPrototypeTokenLock> TokenLocks { get; } = [];
}

public sealed class GridPrototypeException(string message) : Exception(message);
