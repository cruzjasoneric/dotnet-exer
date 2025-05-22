namespace EmployeeBFF.Middleware
{
    public class BffExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BffExceptionMiddleware> _logger;

        public BffExceptionMiddleware(RequestDelegate next, ILogger<BffExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var errorResponse = new { message = "An unexpected error occurred." };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }

}
