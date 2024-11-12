using GitHubApiClient;
using log4net;

namespace Helpers {
    public static class GitHubHelpers {
        public static string GetInput(string str, ILog log) {
            Console.WriteLine(str);
            log.Info(str);
            return Console.ReadLine();
        }

        public static void PrintRepositoryInfo(Repository repo) {
            Console.WriteLine($"Name: {repo.Name}");
            Console.WriteLine($"GitHub: {repo.GitHubHomeUrl}");
            Console.WriteLine();
        }

        public static void PrintCommitInfo(Commit commit) {
            Console.WriteLine($"Commit SHA: {commit.Sha}");
            Console.WriteLine($"Author: {commit.AuthorName()}");
            Console.WriteLine($"Date: {(commit.Date() == DateTime.MinValue ? "Unknown Date" : commit.Date().ToString())}");
            Console.WriteLine($"Message: {commit.Message()}");
            Console.WriteLine();
        }
    }
}