

namespace WebInstallationOfFloorsApplication;

public class ErrorHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly Log.Logger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, Log.Logger logger) {
        _next = next;
        _logger = logger;
    }
    
    public async System.Threading.Tasks.Task InvokeAsync(HttpContext httpContext) {
        try {
            await _next(httpContext);
        }
        catch (Exception ex) {
            _logger.Error($"Произошла ошибка: {ex.Message}");
            _logger.Error($"Стек вызовов: {ex.StackTrace}");
                
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 500;
            var response = new { 
                message = "An unexpected error occurred.", 
                error = ex.Message 
            };
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}