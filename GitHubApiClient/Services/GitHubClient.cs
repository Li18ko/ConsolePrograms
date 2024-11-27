using System.Text.Json;
using System.Net.Http.Headers;
using WebGitHubApplication;


namespace GitHubApiClient {
    public class GitHubClient: IGitHubClient {

        private readonly HttpClient client;

        public GitHubClient(string token) {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/128.0.0.0");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    
    
        public async Task<List<Repository>> GetRepositories() {
            var response = await client.GetAsync("/user/repos");
            if (!response.IsSuccessStatusCode) {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new GitHubApiException(
                    $"GitHub API returned an error while fetching repositories: {errorResponse}",
                    (int)response.StatusCode,
                    errorResponse
                );
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var repositories = await JsonSerializer.DeserializeAsync<List<Repository>>(stream);

            return repositories ?? new List<Repository>();
        }

        public async Task<List<Commit>> GetCommits(string owner, string repo) {
            string url = $"/repos/{owner}/{repo}/commits";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode) {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new GitHubApiException(
                    $"GitHub API returned an error while fetching commits for {owner}/{repo}: {errorResponse}",
                    (int)response.StatusCode,
                    errorResponse
                );
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var commits = await JsonSerializer.DeserializeAsync<List<Commit>>(stream);

            return commits ?? new List<Commit>();
        }
    }
}