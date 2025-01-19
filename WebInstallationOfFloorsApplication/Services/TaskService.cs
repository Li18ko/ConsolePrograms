using Log;

namespace WebInstallationOfFloorsApplication;

public class TaskService {
    private readonly TaskRepository _taskRepository;
    private readonly Logger _logger;

    public TaskService(TaskRepository taskRepository, Logger logger) {
        _taskRepository = taskRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<Task>> GetAllTasksAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка задач");
        var tasks = await _taskRepository.GetAllTasksAsync(cancellationToken);
        _logger.Debug($"Найдено задач: {tasks.Count()}");
        return tasks;
    }
    
    public async Task<Task> GetTaskAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            _logger.Warning("Некорректный id");
            return null;
        }
        logDebugRequestSuccessful($"получение задачи по id = {id}");
        var task = await _taskRepository.GetTaskAsync(id, cancellationToken);

        if (task == null) {
            throw new Exception($"Задача с id = {id} не найдена");
        }
        logDebugActionSuccessful($"найдена c id = {id}");
        return task;
    }
    
    public async Task<int?> InsertTaskAsync(Task task, CancellationToken cancellationToken) {
        if (task == null) {
            _logger.Warning("Некорректные данные задачи в запросе на добавление");
            return null;
        }
        
        logDebugRequestSuccessful("добавление новой задачи");
        var insertTask = await _taskRepository.InsertTaskAsync(task, cancellationToken);
        logDebugActionSuccessful("добавлена");
        return insertTask;
    }
    
    public async Task<Task> UpdateTaskAsync(Task task, CancellationToken cancellationToken) {
        if (task == null || task.Id <= 0) {
            _logger.Warning("Некорректные данные задачи в запросе на обновление");
            return null;
        }
        
        logDebugRequestSuccessful($"обновление данных о задаче c id = {task.Id}");
        var updatedTask = await _taskRepository.GetTaskAsync(task.Id, cancellationToken);
        if (updatedTask == null) {
            throw new Exception($"Задача с id = {task.Id} не найдена");
        }
        
        await _taskRepository.UpdateTaskAsync(task, cancellationToken);
        logDebugActionSuccessful($"найдена c id = {task.Id}");
        
        var getUpdatedTask = await _taskRepository.GetTaskAsync(task.Id, cancellationToken);
        
        return getUpdatedTask;
    }
    
    public async System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            throw new Exception("id должен быть больше нуля");
        }
        logDebugRequestSuccessful($"удаление данных о задаче c id = {id}");
        var deleteTask = await _taskRepository.GetTaskAsync(id, cancellationToken);
        if (deleteTask == null) {
            throw new Exception($"Задача с id = {id} не найдена");
        }
            
        await _taskRepository.DeleteTaskAsync(id, cancellationToken);
        logDebugActionSuccessful($"удалена c id = {id}");
    }
    
    private void logDebugRequestSuccessful(string action) {
        _logger.Debug("Получен запрос на " + action);
    }
    
    private void logDebugActionSuccessful(string action) {
        _logger.Debug("Задача успешно " + action);
    }
}