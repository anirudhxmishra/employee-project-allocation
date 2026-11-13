
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = exception switch
            {
                KeyNotFoundException =>
                    (
                        StatusCodes.Status404NotFound,
                        "Resource Not Found",
                        "The requested resource was not found."
                    ),

                InvalidOperationException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Operation Conflict",
                        exception.Message
                    ),

                ArgumentException =>
                    (
                        StatusCodes.Status400BadRequest,
                        "Invalid Request",
                        "The supplied request contains invalid arguments."
                    ),

                DbUpdateException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Database Conflict",
                        "The request could not be completed due to a database constraint."
                    ),

                _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "Internal Server Error",
                        "An unexpected error occurred. Please try again later."
                    )
            };

            _logger.LogError(
                exception,
                "Unhandled exception occurred. Request: {Path}",
                httpContext.Request.Path);

            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                });
        }
    }
}
