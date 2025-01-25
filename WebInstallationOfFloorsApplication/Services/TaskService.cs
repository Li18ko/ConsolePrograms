using Log;
using Mapster;
using MapsterMapper;

namespace WebInstallationOfFloorsApplication;

public class TaskService {
    private readonly TaskRepository _taskRepository;
    private readonly Logger _logger;
    private readonly IMapper _mapper;

    public TaskService(TaskRepository taskRepository, Logger logger, IMapper mapper) {
        _taskRepository = taskRepository;
        _logger = logger;
        _mapper = mapper;
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
        
        var task = _mapper.Map<Task>(dto);
        
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
        
        updatedTask = _mapper.Map(dto, updatedTask);
        
        await _taskRepository.UpdateTaskAsync(updatedTask, cancellationToken);
        logDebugActionSuccessful($"найдена c id = {updatedTask.Id}");
        
        return _mapper.Map<TaskUpdateDto>(updatedTask);
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