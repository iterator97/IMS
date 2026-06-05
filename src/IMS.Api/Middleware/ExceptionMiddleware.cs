using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using IMS.Application.Validation.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IMS.Api.Middleware
{
    public sealed class ExceptionMiddleware(ILogger<ExceptionMiddleware> _logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException exception)
            {
                await HandleValidationException(context, exception);
            }
            catch (Exception exception)
            {
                await HandleException(context, exception);
            }
        }

        private async Task HandleValidationException(HttpContext context, ValidationException validationExceptions)
        {
            var validationErrors = validationExceptions.Errors?
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray())
                ?? [];

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(validationErrors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "Validation failure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred"
            });
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                traceId);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new ApiErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error",
                TraceId = traceId
            };

            await context.Response.WriteAsJsonAsync(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }
}
