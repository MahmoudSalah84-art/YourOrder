using yourOrder.APIs.Errors;

namespace yourOrder.APIs.Errors
{
    public class ApiExceptionResponse(int statusCode, string? message = null , string? details = null ) : ApiResponse(statusCode, message)
    {
        public string? Details { get; set; } = details;
    }
}

