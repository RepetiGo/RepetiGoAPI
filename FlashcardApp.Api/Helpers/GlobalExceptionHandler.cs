
using Microsoft.AspNetCore.Diagnostics;

namespace FlashcardApp.Api.Helpers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred (GlobalExceptionHandler): {Message}", exception.Message);

            var status = exception switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = status;
            var errorResponse = new
            {
                Title = "An error occurred",
                Status = status,
                Detail = exception.Message,
                Type = exception.GetType().Name,
            };
            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken: cancellationToken);

            return true;
        }
    }
}
