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
    
    public async Task<int?> InsertTaskAsync(TaskInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные задачи в запросе на добавление");
            return null;
        }
        
        var task = new Task {
            Title = dto.Title,
            Deadline = dto.Deadline,
            Comment = dto.Comment,
            Address = dto.Address,
            WorkerId = dto.WorkerId,
            CreatedAt = DateTime.UtcNow.AddHours(3), 
            Status = TaskStatus.Open
        };
        
        logDebugRequestSuccessful("добавление новой задачи");
        var insertTask = await _taskRepository.InsertTaskAsync(task, cancellationToken);
        logDebugActionSuccessful("добавлена");
        return insertTask;
    }
    
    public async Task<TaskUpdateDto> UpdateTaskAsync(TaskUpdateDto dto, CancellationToken cancellationToken) {
        if (dto == null || dto.Id <= 0) {
            _logger.Warning("Некорректные данные задачи в запросе на обновление");
            return null;
        }
        
        logDebugRequestSuccessful($"обновление данных о задаче c id = {dto.Id}");
        var updatedTask = await _taskRepository.GetTaskAsync(dto.Id, cancellationToken);
        if (updatedTask == null) {
            throw new Exception($"Задача с id = {dto.Id} не найдена");
        }
        
        updatedTask.Title = dto.Title;
        updatedTask.Deadline = dto.Deadline;
        updatedTask.Comment = dto.Comment;
        updatedTask.Address = dto.Address;
        updatedTask.WorkerId = dto.WorkerId;
        updatedTask.Status = dto.Status;
        
        await _taskRepository.UpdateTaskAsync(updatedTask, cancellationToken);
        logDebugActionSuccessful($"найдена c id = {updatedTask.Id}");
        
        return new TaskUpdateDto {
            Id = updatedTask.Id,
            Title = updatedTask.Title,
            Deadline = updatedTask.Deadline,
            Comment = updatedTask.Comment,
            Address = updatedTask.Address,
            WorkerId = updatedTask.WorkerId,
            Status = updatedTask.Status
        };
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