using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase {
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService) {
        _taskService = taskService;
    }

    [HttpGet("List")]
    public async Task<IEnumerable<Task>> GetAllTasksAsync(CancellationToken cancellationToken) {
        return await _taskService.GetAllTasksAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<Task> GetTaskAsync(int id, CancellationToken cancellationToken) {
        return await _taskService.GetTaskAsync(id, cancellationToken);
    }

    [HttpPost]
    public async Task<int?> InsertTaskAsync([FromBody] TaskInsertDto dto, CancellationToken cancellationToken) {
        return await _taskService.InsertTaskAsync(dto, cancellationToken);
    }

    [HttpPut]
    public async Task<TaskUpdateDto> UpdateTaskAsync([FromBody] TaskUpdateDto dto, CancellationToken cancellationToken) {
        return await _taskService.UpdateTaskAsync(dto, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken) {
        await _taskService.DeleteTaskAsync(id, cancellationToken);
    }

}