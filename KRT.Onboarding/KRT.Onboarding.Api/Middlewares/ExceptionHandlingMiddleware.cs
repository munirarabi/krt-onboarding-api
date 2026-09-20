using KRT.Onboarding.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace KRT.Onboarding.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;

            // Daria pra melhor usando um Switch Case
            if (exception is NotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (exception is ConflictException)
            {
                // Nesse caso utilizei o Status Code 409 (proprio para Conflito)
                // mas outra opção é retornar como Bad Request também..
                statusCode = HttpStatusCode.Conflict;
            }
            else if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    _logger.LogWarning(
                        "Resource not found. Path: {Path}. Message: {Message}",
                        context.Request.Path,
                        exception.Message);
                    break;

                case HttpStatusCode.Conflict:
                    _logger.LogWarning(
                        "Conflict occurred. Path: {Path}. Message: {Message}",
                        context.Request.Path,
                        exception.Message);
                    break;

                case HttpStatusCode.BadRequest:
                    _logger.LogWarning(
                        "Invalid request. Path: {Path}. Message: {Message}",
                        context.Request.Path,
                        exception.Message);
                    break;

                default:
                    _logger.LogError(
                        exception,
                        "An unexpected error occurred. Path: {Path}",
                        context.Request.Path);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = context.Response.StatusCode,
                message = statusCode == HttpStatusCode.InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message
            };

            var jsonSerialize = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(jsonSerialize);
        }
    }
}
