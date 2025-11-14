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
            var statusCode = GetExceptionStatusCode(exception).statusCode;

            _logger.LogError(exception, exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var error = GetExceptionStatusCode(exception).error;

            if (_environment.IsDevelopment())
            {
                var response = new
                {
                    StatusCode = statusCode,
                    Error = error,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            else
            {
                var response = new
                {
                    StatusCode = statusCode,
                    Error = error,
                    Message = exception.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

    private (int statusCode, string error) GetExceptionStatusCode(Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        return (statusCode, error);
    }
}
