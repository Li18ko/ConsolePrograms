using ILogger = Log.ILogger;

namespace WebGitHubApplication {
    public class ErrorHandlingMiddleware {
        private readonly RequestDelegate _next;
        private readonly Log.Logger _logger;
        
        public ErrorHandlingMiddleware(RequestDelegate next, Log.Logger logger){
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext httpContext) {
            try {
                await _next(httpContext);
            }
            catch (GitHubApiException ex) {
                _logger.Error($"Произошла ошибка: {ex.Message}");
                _logger.Error($"Стек вызовов: {ex.StackTrace}");
                httpContext.Response.StatusCode = ex.StatusCode;
                httpContext.Response.ContentType = "application/json";
                var response = new {
                    message = ex.Message
                };
                await httpContext.Response.WriteAsJsonAsync(response);
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
}
