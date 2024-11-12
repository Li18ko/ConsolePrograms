using GitHubApiClient;
using Helpers;
using log4net;
using log4net.Config;

namespace СonsolePrograms {
    
    public class Program {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static async Task Main(string[] args) {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            
            string[] directories = { 
                @"D:\C#\СonsolePrograms\ConsolePrograms\СonsolePrograms\bin\Debug\net8.0\log\", 
                @"D:\C#\СonsolePrograms\ConsolePrograms\СonsolePrograms\bin\Debug\net8.0\log\errors\" 
            };

            string[] zipPaths = { 
                @"D:\C#\СonsolePrograms\ConsolePrograms\СonsolePrograms\bin\Debug\net8.0\log\archive.zip", 
                @"D:\C#\СonsolePrograms\ConsolePrograms\СonsolePrograms\bin\Debug\net8.0\log\errors\archive.zip" 
            };
            
            for (int i = 0; i < directories.Length; i++) {
                LogArchiver.ArchiveLogs(directories[i], zipPaths[i]);
            }
            
            log.Info("Начало");
            
            string token = GitHubHelpers.GetInput("Введите токен: ", log);

            try {
                log.Debug("Создание экземпляра GitHubClient");
                GitHubClient gitHubClient = new GitHubClient(token);
                
                log.Debug("Запрашиване списка репозиториев");
                var repositories = await gitHubClient.GetRepositories();
                log.Debug("Http запрос успешен, список репозиториев получен");
                
                foreach (var repo in repositories)
                {
                    GitHubHelpers.PrintRepositoryInfo(repo);
                }

                string repositoryName = GitHubHelpers.GetInput("Введите имя репозитория: ", log);
                    
                try {
                    log.Debug($"Поиск репозитория '{repositoryName}'");
                    var selectedRepo = repositories.FirstOrDefault(repo =>
                        repo.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase));
                    
                    log.Debug($"Репозиторий существует: {selectedRepo.Name}");

                    log.Debug($"Запрашивание списка коммитов репозитория '{repositoryName}'");
                    var commits = await gitHubClient.GetCommits(selectedRepo.Owner.Login, selectedRepo.Name);
                    log.Debug("Коммиты найдены");
                    foreach (var commit in commits) {
                        GitHubHelpers.PrintCommitInfo(commit);
                    }
                }
                catch (HttpRequestException ex) {
                    log.Error("Ошибка при HTTP-запросе: " + ex);
                }
                catch (NullReferenceException ex) {
                    log.Error($"Репозитория с именем '{repositoryName}' не существует, " + ex);
                }
                catch (Exception ex) {
                    log.Error("Произошла непредвиденная ошибка: " + ex);
                }
                
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401 (Unauthorized)")) {
                log.Error("Ошибка 401: Неверный или недействительный токен, " + ex);
            }
            catch (Exception ex) {
                log.Error("Произошла непредвиденная ошибка: " + ex);
            }
            finally {
                log.Info("Программа завершена");
            }
        }

    }
}

