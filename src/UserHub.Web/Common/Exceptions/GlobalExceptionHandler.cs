using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Web.Common.Exceptions;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail, code, errors) = Map(exception);

        if (status >= 500)
            logger.LogError(exception, "Unhandled exception at {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://userhub.example.com/errors/{title.ToLowerInvariant().Replace(' ', '-')}"
        };

        if (code is not null) problem.Extensions["code"] = code;
        if (errors is not null) problem.Extensions["errors"] = errors;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }

    private static (int Status, string Title, string Detail, string? Code, object? Errors) Map(Exception ex) =>
        ex switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "One or more validation errors occurred. See 'errors' for details.",
                null,
                ve.Errors
                    .GroupBy(e => char.ToLowerInvariant(e.PropertyName[0]) + e.PropertyName[1..])
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            NotFoundException nf => (StatusCodes.Status404NotFound, "Resource not found", nf.Message, nf.Code, null),
            ConflictException cf => (StatusCodes.Status409Conflict, "Resource conflict", cf.Message, cf.Code, null),
            ForbiddenException fb => (StatusCodes.Status403Forbidden, "Access denied", fb.Message, fb.Code, null),

            _ => (StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred. Please contact support with the traceId.", null, null)
        };
}