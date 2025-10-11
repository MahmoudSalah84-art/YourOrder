using System.Text.Json;
using yourOrder.APIs.Errors;

namespace yourOrder.APIs.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Tries to execute the next middleware/endpoint
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // Log the exception

                // Format the response
                context.Response.ContentType = "application/json"; // Set content type to JSON
                context.Response.StatusCode = 500;

                var response = _env.IsDevelopment()
                    ? new ApiExceptionResponse(500, ex.Message, ex.StackTrace.ToString())
                    : new ApiExceptionResponse(500);

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json); // Write the JSON response
            }
        }
    }
}
