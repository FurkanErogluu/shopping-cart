public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int Status { get; set; }
    public T? Payload { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse<T> Ok(T payload, int status = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Status = status,
            Payload = payload,
            Error = null
        };
    }

    public static ApiResponse<T> Fail(string code, string message, int status)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Status = status,
            Payload = default,
            Error = new ApiError(code, message)
        };
    }
}