using global::Voyara.API.Middleware;
using Voyara.API.Middleware;
namespace Voyara.API.Extensions
{
   

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(
            this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionMiddleware>();

        public static IApplicationBuilder UseRateLimiting(
            this IApplicationBuilder app)
            => app.UseMiddleware<RateLimitMiddleware>();

    }
}
