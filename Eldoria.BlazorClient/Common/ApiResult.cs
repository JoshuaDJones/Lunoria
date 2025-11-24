namespace Eldoria.BlazorClient.Common
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }

        public static ApiResult<T> Ok(T data) =>
            new ApiResult<T> { Success = true, Data = data };

        public static ApiResult<T> Fail(string error) => 
            new ApiResult<T> { Success = false, Error = error };
    }
}
