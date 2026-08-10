using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Eldoria.Api.GridPrototype;

public sealed partial class GridPrototypeSessionStore
{
    public const int DefaultRows = 20;
    public const int DefaultColumns = 36;
    public const int MaximumDimension = 100;

    private const int MaximumBackgroundLength = 3_500_000;
    private const int MaximumTextLength = 250;
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);
    private static readonly TimeSpan TokenLockLifetime = TimeSpan.FromSeconds(8);
    private static readonly char[] CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

    private readonly ConcurrentDictionary<string, GridPrototypeSession> _sessions =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _connectionSessions = [];

    public CreateGridPrototypeSessionResult Create(string connectionId) =>
        Create(connectionId, DefaultRows, DefaultColumns, "#ffffff", null);

    public CreateGridPrototypeSessionResult Create(
        string connectionId,
        int rows,
        int columns,
        string gridColor,
        string? backgroundImage)
    {
        RemoveExpiredSessions();

        if (rows is < 1 or > MaximumDimension || columns is < 1 or > MaximumDimension)
            throw new GridPrototypeException("Grid rows and columns must each be between 1 and 100.");
        if (!HexColorRegex().IsMatch(gridColor))
            throw new GridPrototypeException("The grid color must be a hexadecimal color.");

        var normalizedBackground = string.IsNullOrWhiteSpace(backgroundImage)
            ? null
            : backgroundImage.Trim();
        if (normalizedBackground?.Length > MaximumBackgroundLength ||
            normalizedBackground is not null &&
            !normalizedBackground.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) &&
            !Uri.TryCreate(normalizedBackground, UriKind.Absolute, out _))
            throw new GridPrototypeException("The background image is invalid or too large.");

        GridPrototypeSession session;
        do
        {
            session = new GridPrototypeSession
            {
                Code = GenerateCode(),
                HostToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
                Rows = rows,
                Columns = columns,
                GridColor = gridColor.ToLowerInvariant(),
                BackgroundImage = normalizedBackground,
                ExpiresAt = DateTime.UtcNow.Add(SessionLifetime),
            };
        }
        while (!_sessions.TryAdd(session.Code, session));

        _connectionSessions[connectionId] = session.Code;
        return new CreateGridPrototypeSessionResult(session.HostToken, Snapshot(session));
    }

    public GridPrototypeSessionSnapshot Join(string connectionId, string code)
    {
        RemoveExpiredSessions();
        var session = GetSession(code);

        lock (session.SyncRoot)
        {
            Touch(session);
            _connectionSessions[connectionId] = session.Code;
            return SnapshotUnsafe(session);
        }
    }

    public string? GetJoinedCode(string connectionId) =>
        _connectionSessions.TryGetValue(connectionId, out var code) ? code : null;

    public bool BeginMove(string connectionId, string code, string tokenId)
    {
        var session = GetJoinedSession(connectionId, code);
        lock (session.SyncRoot)
        {
            if (!session.Tokens.ContainsKey(tokenId))
                throw new GridPrototypeException("That token is no longer on the board.");

            RemoveExpiredLocks(session);
            if (session.TokenLocks.TryGetValue(tokenId, out var existing) &&
                existing.ConnectionId != connectionId)
                return false;

            session.TokenLocks[tokenId] = new GridPrototypeTokenLock(
                connectionId, DateTime.UtcNow.Add(TokenLockLifetime));
            Touch(session);
            return true;
        }
    }

    public GridPrototypeSessionSnapshot Move(
        string connectionId, string code, string tokenId, int row, int column)
    {
        var session = GetJoinedSession(connectionId, code);
        lock (session.SyncRoot)
        {
            if (!session.Tokens.TryGetValue(tokenId, out var token))
                throw new GridPrototypeException("That token is no longer on the board.");

            RemoveExpiredLocks(session);
            if (session.TokenLocks.TryGetValue(tokenId, out var existing) &&
                existing.ConnectionId != connectionId)
                throw new GridPrototypeException("Another participant is moving that token.");

            session.Tokens[tokenId] = token with
            {
                Row = Math.Clamp(row, 0, session.Rows - 1),
                Column = Math.Clamp(column, 0, session.Columns - 1),
            };
            session.TokenLocks.Remove(tokenId);
            Touch(session);
            return SnapshotUnsafe(session);
        }
    }

    public void EndMove(string connectionId, string code, string tokenId)
    {
        var session = GetJoinedSession(connectionId, code);
        lock (session.SyncRoot)
        {
            if (session.TokenLocks.TryGetValue(tokenId, out var existing) &&
                existing.ConnectionId == connectionId)
                session.TokenLocks.Remove(tokenId);
        }
    }

    public GridPrototypeSessionSnapshot AddToken(
        string code, string hostToken, GridPrototypeCharacterDto character)
    {
        var session = GetHostSession(code, hostToken);
        if (character.Id <= 0 || string.IsNullOrWhiteSpace(character.Name))
            throw new GridPrototypeException("The character is invalid.");
        if (character.Name.Length > MaximumTextLength || character.ImageUrl.Length > MaximumBackgroundLength)
            throw new GridPrototypeException("The character data is too large.");

        lock (session.SyncRoot)
        {
            var id = Guid.NewGuid().ToString("N");
            session.Tokens[id] = new GridPrototypeTokenDto(
                id, character.Id, character.Name.Trim(), character.ImageUrl,
                (session.Rows - 1) / 2, (session.Columns - 1) / 2);
            Touch(session);
            return SnapshotUnsafe(session);
        }
    }

    public GridPrototypeSessionSnapshot RemoveToken(string code, string hostToken, string tokenId)
    {
        var session = GetHostSession(code, hostToken);
        lock (session.SyncRoot)
        {
            session.Tokens.Remove(tokenId);
            session.TokenLocks.Remove(tokenId);
            Touch(session);
            return SnapshotUnsafe(session);
        }
    }

    public GridPrototypeSessionSnapshot SetBackground(
        string code, string hostToken, string? backgroundImage)
    {
        var session = GetHostSession(code, hostToken);
        var normalized = string.IsNullOrWhiteSpace(backgroundImage) ? null : backgroundImage.Trim();
        if (normalized?.Length > MaximumBackgroundLength ||
            normalized is not null &&
            !normalized.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) &&
            !Uri.TryCreate(normalized, UriKind.Absolute, out _))
            throw new GridPrototypeException("The background image is invalid or too large.");

        lock (session.SyncRoot)
        {
            session.BackgroundImage = normalized;
            Touch(session);
            return SnapshotUnsafe(session);
        }
    }

    public GridPrototypeSessionSnapshot SetGridColor(string code, string hostToken, string gridColor)
    {
        var session = GetHostSession(code, hostToken);
        if (!HexColorRegex().IsMatch(gridColor))
            throw new GridPrototypeException("The grid color must be a hexadecimal color.");

        lock (session.SyncRoot)
        {
            session.GridColor = gridColor.ToLowerInvariant();
            Touch(session);
            return SnapshotUnsafe(session);
        }
    }

    public string Close(string code, string hostToken)
    {
        var session = GetHostSession(code, hostToken);
        _sessions.TryRemove(session.Code, out _);
        foreach (var connection in _connectionSessions.Where(item =>
                     string.Equals(item.Value, session.Code, StringComparison.OrdinalIgnoreCase)))
            _connectionSessions.TryRemove(connection.Key, out _);
        return session.Code;
    }

    public string? Disconnect(string connectionId)
    {
        if (!_connectionSessions.TryRemove(connectionId, out var code) ||
            !_sessions.TryGetValue(code, out var session))
            return code;

        lock (session.SyncRoot)
        {
            foreach (var tokenId in session.TokenLocks
                         .Where(item => item.Value.ConnectionId == connectionId)
                         .Select(item => item.Key).ToList())
                session.TokenLocks.Remove(tokenId);
        }
        return code;
    }

    private GridPrototypeSession GetJoinedSession(string connectionId, string code)
    {
        var session = GetSession(code);
        if (!_connectionSessions.TryGetValue(connectionId, out var joinedCode) ||
            !string.Equals(joinedCode, session.Code, StringComparison.OrdinalIgnoreCase))
            throw new GridPrototypeException("Join the session before moving a token.");
        return session;
    }

    private GridPrototypeSession GetHostSession(string code, string hostToken)
    {
        var session = GetSession(code);
        if (string.IsNullOrWhiteSpace(hostToken) ||
            !string.Equals(session.HostToken, hostToken, StringComparison.Ordinal))
            throw new GridPrototypeException("The host token is invalid.");
        return session;
    }

    private GridPrototypeSession GetSession(string code)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (!_sessions.TryGetValue(normalized, out var session) || session.ExpiresAt <= DateTime.UtcNow)
        {
            if (session is not null)
                _sessions.TryRemove(normalized, out _);
            throw new GridPrototypeException("The grid session was not found or has expired.");
        }
        return session;
    }

    private static GridPrototypeSessionSnapshot Snapshot(GridPrototypeSession session)
    {
        lock (session.SyncRoot)
            return SnapshotUnsafe(session);
    }

    private static GridPrototypeSessionSnapshot SnapshotUnsafe(GridPrototypeSession session) =>
        new(session.Code, session.Rows, session.Columns, session.GridColor, session.BackgroundImage,
            session.Tokens.Values.OrderBy(token => token.Name).ThenBy(token => token.Id).ToList());

    private static void Touch(GridPrototypeSession session) =>
        session.ExpiresAt = DateTime.UtcNow.Add(SessionLifetime);

    private static void RemoveExpiredLocks(GridPrototypeSession session)
    {
        var now = DateTime.UtcNow;
        foreach (var tokenId in session.TokenLocks.Where(item => item.Value.ExpiresAt <= now)
                     .Select(item => item.Key).ToList())
            session.TokenLocks.Remove(tokenId);
    }

    private void RemoveExpiredSessions()
    {
        var now = DateTime.UtcNow;
        foreach (var session in _sessions.Where(item => item.Value.ExpiresAt <= now).ToList())
            _sessions.TryRemove(session.Key, out _);
    }

    private static string GenerateCode()
    {
        Span<char> code = stackalloc char[8];
        for (var i = 0; i < code.Length; i++)
            code[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
        return new string(code);
    }

    [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
    private static partial Regex HexColorRegex();
}
