using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OrderCompletion.Api.Infrastructure.Exceptions;

/// <summary>
/// Handle exceptions globally using the built-in Middleware
/// Doing this ensures the stack trace is not sent to the client, ensuring any sensitive info is kept safe
/// Also return problem details for uniformity
/// </summary>
internal sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int status = StatusCodes.Status500InternalServerError;
        
        var problemDetails = new ProblemDetails
        {
            Title = "An internal server error occurred",
            Type = exception.GetType().Name,
            Detail = exception.Message,
            Status = status,
        };
        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}
