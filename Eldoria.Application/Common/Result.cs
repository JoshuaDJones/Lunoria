namespace Eldoria.Application.Common
{
    public sealed record Error(string Code, string Message)
    {
        public static readonly Error None = new("", "");
    }

    public sealed record Result(bool Success, Error Error)
    {
        public static Result Ok() => new(true, Error.None);
        public static Result Fail(Error error) => new(false, error);
    }

    public sealed record Result<T>(bool Success, T? Value, Error Error)
    {
        public static Result<T> Ok(T value) => new(true, value, Error.None);
        public static Result<T> Fail(Error error) => new(false, default, error);
    }
}
