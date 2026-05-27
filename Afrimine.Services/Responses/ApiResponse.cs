using System.Text.Json.Serialization;

namespace Afrimine.Services.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = default!;
        public T? Data { get; set; }

        [JsonIgnore] // ← add this — never exposed in JSON response
        public string? RefreshToken { get; set; }

        protected ApiResponse() { }

        protected ApiResponse(string message, int code)
        {
            Message = message;
            StatusCode = code;
        }

        protected ApiResponse(T data, string message, int code)
        {
            Data = data;
            Message = message;
            StatusCode = code;
            Success = true;
        }

        /// <summary>
        /// OK Response
        /// </summary>
        /// <param name="data"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> Ok(T data, int statusCode = 200, string message = "Successful")
        {
            return new ApiResponse<T>(data, message, statusCode);
        }


        /// <summary>OK Response with internal refresh token (for cookie use)</summary>
        public static ApiResponse<T> Ok(T data, string refreshToken, int statusCode = 200, string message = "Successful")
        {
            return new ApiResponse<T>(data, message, statusCode)
            {
                RefreshToken = refreshToken  // ← stored internally, never sent to client in JSON
            };
        }

        /// <summary>
        /// Failed Request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ApiResponse<T> Fail(string message, int statusCode)
        {
            return new ApiResponse<T>(message, statusCode);
        }
    }
}
