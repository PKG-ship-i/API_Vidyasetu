namespace Vidyasetu_API.DTOs.Response
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }  // HTTP Status Code
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T> { StatusCode = statusCode, Message = message, Data = data };
        }

        public static ApiResponse<T> Fail(string message, int statusCode)
        {
            return new ApiResponse<T> { StatusCode = statusCode, Message = message, Data = default };
        }
    }

}
