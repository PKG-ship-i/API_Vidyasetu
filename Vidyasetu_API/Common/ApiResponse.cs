namespace Vidyasetu_API.Common
{
	public class ApiResponse<T>
	{
		public int StatusCode { get; set; }          // HTTP Status Code
		public bool Success { get; set; }            // true = success, false = error
		public string Message { get; set; } = string.Empty;
		public T? Data { get; set; }

		// For successful responses
		public static ApiResponse<T> CreateSuccess(T data, string message = "Success", int statusCode = 200)
		{
			return new ApiResponse<T>
			{
				StatusCode = statusCode,
				Success = true,
				Message = message,
				Data = data
			};
		}

		// For failed responses
		public static ApiResponse<T> CreateFailure(string message, int statusCode = 400)
		{
			return new ApiResponse<T>
			{
				StatusCode = statusCode,
				Success = false, // ✅ Explicitly mark as failed
				Message = message,
				Data = default
			};
		}
	}
}
