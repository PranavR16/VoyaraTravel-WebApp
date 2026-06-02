using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;
namespace Voyara.API.Middleware
{
    public class RateLimitMiddleware(
      RequestDelegate next,
      IDistributedCache cache,
      IConfiguration config)
    {
        private readonly int _maxRequests =
            config.GetValue<int>("RateLimit:MaxRequests", 100);
        private readonly int _windowSeconds =
            config.GetValue<int>("RateLimit:WindowSeconds", 60);

        // Skip rate limiting for these paths
        private static readonly string[] _skipPaths =
        [
            "/api/payments/webhook",
        "/swagger",
        "/health"
        ];

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();

            // Skip certain paths
            if (_skipPaths.Any(p => path.StartsWith(p)))
            {
                await next(context);
                return;
            }

            var ip = GetClientIp(context);
            var key = $"ratelimit:{ip}:{path.Split('/').Take(3).Last()}";

            try
            {
                var countStr = await cache.GetStringAsync(key);
                var count = countStr != null ? int.Parse(countStr) : 0;

                if (count >= _maxRequests)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Append("Retry-After", _windowSeconds.ToString());
                    context.Response.Headers.Append("X-RateLimit-Limit",
                        _maxRequests.ToString());
                    context.Response.Headers.Append("X-RateLimit-Remaining", "0");

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        statusCode = 429,
                        message = "Too many requests. Please try again later.",
                        retryAfter = _windowSeconds
                    }));
                    return;
                }

                // Increment counter
                await cache.SetStringAsync(
                    key,
                    (count + 1).ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromSeconds(_windowSeconds)
                    });

                // Add rate limit headers
                context.Response.Headers.Append("X-RateLimit-Limit",
                    _maxRequests.ToString());
                context.Response.Headers.Append("X-RateLimit-Remaining",
                    (_maxRequests - count - 1).ToString());
            }
            catch
            {
                // If Redis is down, allow request through
            }

            await next(context);
        }

        private static string GetClientIp(HttpContext context)
        {
            // Check for proxy forwarded IP first
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
                return forwarded.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    //public class RateLimitMiddleware(RequestDelegate next, IDistributedCache cache)
    //{
    //    private const int MaxRequests = 100;
    //    private const int WindowSeconds = 60;

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    //        var key = $"ratelimit:{ip}";

    //        var countStr = await cache.GetStringAsync(key);
    //        var count = countStr != null ? int.Parse(countStr) : 0;

    //        if (count >= MaxRequests)
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
    //            context.Response.ContentType = "application/json";
    //            await context.Response.WriteAsync(
    //                "{\"message\":\"Too many requests. Please slow down.\"}");
    //            return;
    //        }

    //        await cache.SetStringAsync(key, (count + 1).ToString(),
    //            new DistributedCacheEntryOptions
    //            {
    //                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(WindowSeconds)
    //            });

    //        await next(context);
    //    }
    //}
}
