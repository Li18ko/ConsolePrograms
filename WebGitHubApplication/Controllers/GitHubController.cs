using GitHubApiClient;
using Microsoft.AspNetCore.Mvc;

namespace WebGitHubApplication {
    [ApiController]
    [Route("api/github")]
    public class GitHubController: ControllerBase {
    
        private readonly IGitHubClient _gitHubClient;

        public GitHubController(IGitHubClient gitHubClient) {
            _gitHubClient = gitHubClient;
        }

        [HttpGet("repositories")]
        public async Task<IActionResult> GetRepositories() {
            var repositories = await _gitHubClient.GetRepositories();
            return Ok(repositories);
        }

        [HttpGet("repositories/{owner}/{repo}/commits")]
        public async Task<IActionResult> GetCommits(string owner, string repo) {
            var commits = await _gitHubClient.GetCommits(owner, repo);
            return Ok(commits);
        }
    }
}