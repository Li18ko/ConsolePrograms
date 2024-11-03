using System;

namespace GitHubApiClient {
    public class Program {
        public static async Task Main(string[] args) {
            Console.WriteLine("Введите токен: ");
            string token = Console.ReadLine();
            GitHubClient gitHubClient = new GitHubClient(token);
            
            var repositories = await gitHubClient.GetRepositories();
            
            
            foreach (var repo in repositories)
            {
                Console.WriteLine($"Name: {repo.Name}");
                Console.WriteLine($"GitHub: {repo.GitHubHomeUrl}");
                Console.WriteLine();
            }

            Console.Write("Введите имя репозитория: ");
            string repositoryName = Console.ReadLine();
            var selectedRepo = repositories.FirstOrDefault(repo => repo.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase));

            if (selectedRepo != null) {
                Console.WriteLine($"Репозиторий: {selectedRepo.Name}");
                Console.WriteLine();
    
                var commits = await gitHubClient.GetCommits(selectedRepo.Owner.Login, selectedRepo.Name);
                foreach (var commit in commits){
                    Console.WriteLine($"Commit SHA: {commit.Sha}");
                    Console.WriteLine($"Author: {commit.AuthorName()}");
                    Console.WriteLine($"Date: {(commit.Date() == DateTime.MinValue ? "Unknown Date" : commit.Date().ToString())}");
                    Console.WriteLine($"Message: {commit.Message()}");
                    Console.WriteLine();
                }

    
            }
            else {
                Console.WriteLine("Репозиторий с таким именем не найден.");
            }
            
        }
    }

}