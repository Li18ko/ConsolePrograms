namespace WebGitHubApplication {
    public class ErrorHandlingMiddleware {
        private readonly RequestDelegate _next;
        
        public ErrorHandlingMiddleware(RequestDelegate next){
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext httpContext) {
            try {
                await _next(httpContext); 
            }
            catch (Exception ex)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";

                response.StatusCode = 500;
                var errorResponse = new {
                    details = ex.Message,
                    stackTrace = ex.StackTrace
                };
                await response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
