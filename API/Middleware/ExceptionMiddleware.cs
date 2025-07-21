
using System.Text.Json;
using Application.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env) {
        _logger = logger;
          _env = env;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }

        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var response = _env.IsDevelopment()
           ? new AppExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
           : new AppExceptions(context.Response.StatusCode, ex.Message, null);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response,options);
            await context.Response.WriteAsync(json);
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (ex.Errors is not null)
            {
                foreach (var error in ex.Errors)
                {
                    if (validationErrors.TryGetValue(error.PropertyName, out var existingErrors))
                    {
                        validationErrors[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                    }
                    else
                    {
                        validationErrors[error.PropertyName] = [ error.ErrorMessage ];
                    }

                }
            }
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var validationProblemDetails = new ValidationProblemDetails(validationErrors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation Error ",
                Detail = "One or More validation errors has occurred"
            };
            await context.Response.WriteAsJsonAsync(validationProblemDetails);
        }
    }
}