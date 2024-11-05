using GitHubApiClient;
using log4net;
using System.Reflection;
using Log;
using log4net.Config;

namespace СonsolePrograms {
    public class Program {
        static async Task Main(string[] args) {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            var demo = new Logger();
            demo.Info("Начало");
            
            Console.WriteLine("Введите токен: ");
            demo.Info("Ввод токена");
            string token = Console.ReadLine();

            try {
                demo.Debug("Создание экземпляра GitHubClient");
                GitHubClient gitHubClient = new GitHubClient(token);
                
                demo.Debug("Запрашиване списка репозиториев");
                var repositories = await gitHubClient.GetRepositories();
                demo.Debug("Http запрос успешен, список репозиториев получен");
                
                foreach (var repo in repositories)
                {
                    Console.WriteLine($"Name: {repo.Name}");
                    Console.WriteLine($"GitHub: {repo.GitHubHomeUrl}");
                    Console.WriteLine();
                }

                Console.Write("Введите имя репозитория: ");
                string repositoryName = Console.ReadLine();

                try {
                    demo.Debug($"Поиск репозитория '{repositoryName}'");
                    var selectedRepo = repositories.FirstOrDefault(repo =>
                        repo.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase));
                    
                    demo.Debug($"Репозиторий существует: {selectedRepo.Name}");

                    demo.Debug($"Запрашивание списка коммитов репозитория '{repositoryName}'");
                    var commits = await gitHubClient.GetCommits(selectedRepo.Owner.Login, selectedRepo.Name);
                    demo.Debug("Коммиты найдены");
                    foreach (var commit in commits) {
                        Console.WriteLine($"Commit SHA: {commit.Sha}");
                        Console.WriteLine($"Author: {commit.AuthorName()}");
                        Console.WriteLine(
                            $"Date: {(commit.Date() == DateTime.MinValue ? "Unknown Date" : commit.Date().ToString())}");
                        Console.WriteLine($"Message: {commit.Message()}");
                        Console.WriteLine();
                    }
                }
                catch (HttpRequestException ex) {
                    demo.Error("Ошибка при HTTP-запросе: " + ex.Message);
                    demo.Error("StackTrace: " + ex.StackTrace);
                }
                catch (NullReferenceException ex) {
                    demo.Error($"Репозитория с именем '{repositoryName}' не существует");
                }
                catch (Exception ex) {
                    demo.Error("Произошла непредвиденная ошибка: " + ex.Message);
                    demo.Error("StackTrace: " + ex.StackTrace);
                }
                
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401 (Unauthorized)")) {
                demo.Error("Ошибка 401: Неверный или недействительный токен.");
            }
            catch (Exception ex) {
                demo.Error("Произошла непредвиденная ошибка: " + ex.Message);
                demo.Error("StackTrace: " + ex.StackTrace);
            }
            finally {
                demo.Info("Программа завершена");
            }
        }

    }
}

