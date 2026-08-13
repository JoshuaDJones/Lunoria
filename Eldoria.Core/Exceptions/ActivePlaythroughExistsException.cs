namespace Eldoria.Core.Exceptions;

public sealed class ActivePlaythroughExistsException : Exception
{
    public ActivePlaythroughExistsException()
        : base("The journey already has an active playthrough.") { }

    public ActivePlaythroughExistsException(Exception innerException)
        : base("The journey already has an active playthrough.", innerException) { }
}
