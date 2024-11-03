namespace GitHubApiClient {
    public interface IGitHubClient {

        Task<List<Repository>> GetRepositories();

        Task<List<Commit>> GetCommits(string owner, string repo);


    }
}
