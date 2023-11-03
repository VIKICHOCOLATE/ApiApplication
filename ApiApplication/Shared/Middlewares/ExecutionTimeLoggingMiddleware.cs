using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiApplication.Shared.Middlewares
{
    public class ExecutionTimeLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExecutionTimeLoggingMiddleware> _logger;

        public ExecutionTimeLoggingMiddleware(RequestDelegate next, ILogger<ExecutionTimeLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var watch = Stopwatch.StartNew();
            await _next(context);
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation($"Request to {context.Request.Path} took {elapsedMs}ms");
        }
    }
}
