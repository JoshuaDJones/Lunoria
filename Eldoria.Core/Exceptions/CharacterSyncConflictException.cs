namespace Eldoria.Core.Exceptions;

public sealed class CharacterSyncConflictException(Exception inner)
    : Exception("Character data changed during this operation. Reload and review again.", inner);

