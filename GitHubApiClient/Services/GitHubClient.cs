using System.Text.Json;
using System.Net.Http.Headers;


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
            await using Stream stream =
                await client.GetStreamAsync("/user/repos");
            var repositories =
                await JsonSerializer.DeserializeAsync<List<Repository>>(stream);
            return repositories ?? new();
        }

        public async Task<List<Commit>> GetCommits(string owner, string repo) {
            string url = $"/repos/{owner}/{repo}/commits";
            await using Stream stream = await client.GetStreamAsync(url);

            var commits = await JsonSerializer.DeserializeAsync<List<Commit>>(stream);
            return commits ?? new();
        }
    }
}