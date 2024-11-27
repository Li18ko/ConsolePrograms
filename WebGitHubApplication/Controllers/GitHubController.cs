using GitHubApiClient;
using Microsoft.AspNetCore.Mvc;

namespace WebGitHubApplication {
    [ApiController]
    [Route("api/github")]
    public class GitHubController: ControllerBase {
    
        private readonly IGitHubClient _gitHubClient;
        private readonly Log.Logger _logger;

        public GitHubController(IGitHubClient gitHubClient, Log.Logger logger) {
            _gitHubClient = gitHubClient;
            _logger = logger;
        }

        [HttpGet("repositories")]
        public async Task<IActionResult> GetRepositories() {
            _logger.Info("Получен запрос на получение списка репозиториев.");
            var repositories = await _gitHubClient.GetRepositories();
            _logger.Info($"Найдено репозиториев: {repositories.Count}");
            return Ok(repositories);
        }

        [HttpGet("repositories/{owner}/{repo}/commits")]
        public async Task<IActionResult> GetCommits(string owner, string repo) {
            _logger.Info($"Получен запрос на получение коммитов для репозитория {owner}/{repo}.");
            var commits = await _gitHubClient.GetCommits(owner, repo);
            _logger.Info($"Найдено коммитов: {commits.Count}");
            return Ok(commits);
        }
    }
}