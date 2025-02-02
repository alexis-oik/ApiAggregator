using ApiAggregator.Errors;
using ApiAggregator.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace ApiAggregator.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occured: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "An error occured",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message,
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(Result.Failure(GlobalErrors.GlobalExceptionError(problemDetails)), cancellationToken);

            return true;
        }
    }
}
