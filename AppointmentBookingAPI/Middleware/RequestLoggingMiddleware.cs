using System.Diagnostics;

namespace AppointmentBookingAPI.Middleware
{
    /// <summary>
    /// Middleware for logging HTTP requests
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            _logger.LogInformation(
                "Incoming Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                _logger.LogInformation(
                    "Completed Request: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
