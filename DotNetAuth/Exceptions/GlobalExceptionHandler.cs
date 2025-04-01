using System.Net;
using DotNetAuth.Domain.Contracts;
using Microsoft.AspNetCore.Diagnostics;

namespace DotNetAuth.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
           _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);
            var response = new ErrorResponse
            {
                Message = exception.Message
            };

            switch(exception)
            {
                case BadHttpRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Title = exception.GetType().Name;
                    break;
          default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Title = "Internal Server Error";
                    break;
            }
            httpContext.Response.StatusCode = response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(response,cancellationToken);
            return true;
        }
    }
}
