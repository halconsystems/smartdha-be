using Azure.Core;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DHAFacilitationAPIs.Web.Infrastructure;

public sealed class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Application.Common.Exceptions.ValidationException exception)
        {
            var problemDetails = new ProblemDetails();
            //{
            //    Status = StatusCodes.Status400BadRequest,
            //    Type = "ValidationFailure",
            //    Title = "Validation error",
            //    Detail = "One or more validation errors has occurred"
            //};

            if (exception.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exception.Errors;
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (UnAuthorizedException exception)
        {
            var problemDetails = new ProblemDetails();
            if (exception is not null)
            {
                problemDetails.Extensions["error"] = exception.Message;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception exception)
        {
            var problemDetails = new ProblemDetails();
            if (exception is not null)
            {
                problemDetails.Extensions["error"] = exception.Message;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
