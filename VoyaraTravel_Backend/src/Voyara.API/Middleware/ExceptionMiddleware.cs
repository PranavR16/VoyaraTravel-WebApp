using System.Net;
using System.Text.Json;
using Voyara.Shared.Exceptions;

namespace Voyara.API.Middleware
{
    public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode;
            string message;

            switch (ex)
            {
                case NotFoundException e:
                    statusCode = HttpStatusCode.NotFound;
                    message = e.Message;
                    break;

                case BadRequestException e:
                    statusCode = HttpStatusCode.BadRequest;
                    message = e.Message;
                    break;

                case UnauthorizedException e:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = e.Message;
                    break;

                case UnauthorizedAccessException e:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = e.Message;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred. Please try again.";
                    logger.LogError(ex,
                        "Unhandled exception: {Message} | Path: {Path}",
                        ex.Message,
                        context.Request.Path);
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = (int)statusCode,
                message,
                path = context.Request.Path.ToString(),
                timestamp = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }

        //public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        //{
        //    public async Task InvokeAsync(HttpContext context)
        //    {
        //        try { await next(context); }
        //        catch (Exception ex) { await HandleAsync(context, ex, logger); }
        //    }

        //    private static async Task HandleAsync(HttpContext ctx, Exception ex, ILogger logger)
        //    {
        //        var (status, message) = ex switch
        //        {
        //            NotFoundException e => (HttpStatusCode.NotFound, e.Message),
        //            BadRequestException e => (HttpStatusCode.BadRequest, e.Message),
        //            UnauthorizedException e => (HttpStatusCode.Unauthorized, e.Message),
        //            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        //        };

        //        if (status == HttpStatusCode.InternalServerError)
        //            logger.LogError(ex, "Unhandled exception");

        //        ctx.Response.StatusCode = (int)status;
        //        ctx.Response.ContentType = "application/json";

        //        await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
        //        {
        //            statusCode = (int)status,
        //            message,
        //            timestamp = DateTime.UtcNow
        //        }));
        //    }
    }

}
