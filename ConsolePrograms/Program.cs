using ConsolePrograms;
using GitHubApiClient;
using Helpers;
using log4net;
using log4net.Config;

namespace СonsolePrograms {
    
    public class Program {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static async Task Main(string[] args) {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            log.Info("Начало");
            
            string token = GitHubHelpers.GetInput("Введите токен: ", log);
            
            if (string.IsNullOrEmpty(token)) {
                log.Error("Токен не был введен.");
                throw new ArgumentException("Токен не был введен");
            }

            try {
                log.Debug("Создание экземпляра GitHubClient");
                GitHubClient gitHubClient = new GitHubClient(token);
                
                log.Debug("Запрашиване списка репозиториев");
                var repositories = await gitHubClient.GetRepositories();
                log.Debug("Http запрос успешен, список репозиториев получен");
                
                foreach (var repo in repositories){
                    GitHubHelpers.PrintRepositoryInfo(repo);
                }

                string repositoryName = GitHubHelpers.GetInput("Введите имя репозитория: ", log);
                if (string.IsNullOrEmpty(repositoryName)) {
                    log.Error("Имя репозитория не было введено");
                    throw new ArgumentException("Имя репозитория не было введено");
                }
                    
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
            catch (HttpRequestException ex) when (ex.Message.Contains("401 (Unauthorized)")) {
                log.Error("Ошибка 401: Неверный или недействительный токен, " + ex.Message);
                throw new UnauthorizedAccessException("Ошибка 401: Неверный или недействительный токен");
            }
            catch (HttpRequestException ex) {
                log.Error("Ошибка при HTTP-запросе: " + ex.Message);
                throw;
            }
            catch (NullReferenceException ex) {
                log.Error($"Репозитория с таким именем не существует, " + ex.Message);
                throw;
            }
            catch (Exception ex) {
                log.Error("Произошла непредвиденная ошибка: " + ex.Message);
                throw;
            }
            finally {
                log.Info("Программа завершена");
            }
        }
    }
}

