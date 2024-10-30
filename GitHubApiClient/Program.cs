using System.Net.Http.Headers;
using System.Text.Json;
using GitHubApiClient;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
client.DefaultRequestHeaders.Add("User-Agent", "GitHubApiClient/1.0");

Console.WriteLine("Введите токен: ");
string token = Console.ReadLine();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

var repositories = await ProcessRepositoriesAsync(client);
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

    var commits = await GetCommitsAsync(client, selectedRepo.Owner.Login, selectedRepo.Name);
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


static async Task<List<Repository>> ProcessRepositoriesAsync(HttpClient client)
{
    await using Stream stream =
        await client.GetStreamAsync("https://api.github.com/user/repos");
    var repositories =
        await JsonSerializer.DeserializeAsync<List<Repository>>(stream);
    return repositories ?? new();
}


static async Task<List<Commit>> GetCommitsAsync(HttpClient client, string owner, string repo)
{
    string url = $"https://api.github.com/repos/{owner}/{repo}/commits";
    await using Stream stream = await client.GetStreamAsync(url);

    var commits = await JsonSerializer.DeserializeAsync<List<Commit>>(stream);
    return commits ?? new();
}