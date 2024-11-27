namespace WebGitHubApplication {
    
    public class GitHubApiException: Exception {
        public int StatusCode { get; set; }
        public string GitHubError { get; set; }

        public GitHubApiException(string message, Exception innerException, int statusCode, string gitHubError = null) : base(message, innerException) {
            StatusCode = statusCode;
            GitHubError = gitHubError;
        }
        
        public GitHubApiException(string message, int statusCode, string gitHubError = null) : base(message) {
            StatusCode = statusCode;
            GitHubError = gitHubError;
        }
    
    }
}