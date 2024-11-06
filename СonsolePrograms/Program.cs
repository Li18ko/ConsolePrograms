using System.Reflection;
using GitHubApiClient;
using Helpers;
using Log;
using log4net;
using log4net.Config;

namespace СonsolePrograms {
    public class Program {
        static async Task Main(string[] args) {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            
            var demo = new Logger();
            demo.Info("Начало");
            
            string token = GitHubHelpers.GetInput("Введите токен: ", demo);

            try {
                demo.Debug("Создание экземпляра GitHubClient");
                GitHubClient gitHubClient = new GitHubClient(token);
                
                demo.Debug("Запрашиване списка репозиториев");
                var repositories = await gitHubClient.GetRepositories();
                demo.Debug("Http запрос успешен, список репозиториев получен");
                
                foreach (var repo in repositories)
                {
                    GitHubHelpers.PrintRepositoryInfo(repo);
                }

                string repositoryName = GitHubHelpers.GetInput("Введите имя репозитория: ", demo);

                try {
                    demo.Debug($"Поиск репозитория '{repositoryName}'");
                    var selectedRepo = repositories.FirstOrDefault(repo =>
                        repo.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase));
                    
                    demo.Debug($"Репозиторий существует: {selectedRepo.Name}");

                    demo.Debug($"Запрашивание списка коммитов репозитория '{repositoryName}'");
                    var commits = await gitHubClient.GetCommits(selectedRepo.Owner.Login, selectedRepo.Name);
                    demo.Debug("Коммиты найдены");
                    foreach (var commit in commits) {
                        GitHubHelpers.PrintCommitInfo(commit);
                    }
                }
                catch (HttpRequestException ex) {
                    demo.Error("Ошибка при HTTP-запросе: " + ex);
                }
                catch (NullReferenceException ex) {
                    demo.Error($"Репозитория с именем '{repositoryName}' не существует, " + ex);
                }
                catch (Exception ex) {
                    demo.Error("Произошла непредвиденная ошибка: " + ex);
                }
                
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401 (Unauthorized)")) {
                demo.Error("Ошибка 401: Неверный или недействительный токен, " + ex);
            }
            catch (Exception ex) {
                demo.Error("Произошла непредвиденная ошибка: " + ex);
            }
            finally {
                demo.Info("Программа завершена");
            }
        }

    }
}

