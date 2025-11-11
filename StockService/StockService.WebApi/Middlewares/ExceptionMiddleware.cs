namespace StockService.WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var statusCode = GetExceptionStatusCode(exception);

            _logger.LogError(exception, exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            if (_environment.IsDevelopment())
            {
                var response = new
                {
                    StatusCode = statusCode,
                    Message = exception.Message,
                    OccurredAt = DateTime.UtcNow,
                    StackTrace = exception.StackTrace
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            else
            {
                var response = new
                {
                    StatusCode = statusCode,
                    Message = exception.Message,
                    OccurredAt = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

    private int GetExceptionStatusCode(Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        return statusCode;
    }
}
