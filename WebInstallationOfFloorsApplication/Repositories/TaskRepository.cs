using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class TaskRepository {
    private readonly AppDbContext _context;
    public TaskRepository(AppDbContext context) {
        _context = context;
    }
    
    public async Task<IEnumerable<Task>> GetAllTasksAsync(CancellationToken cancellationToken) {
        return await _context.Task.ToListAsync(cancellationToken);
    }

    public async Task<Task> GetTaskAsync(int id, CancellationToken cancellationToken) {
        return await _context.Task.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<int?> InsertTaskAsync(Task task, CancellationToken cancellationToken) {
        await _context.Task.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return task.Id;
    }

    public async Task<Task> UpdateTaskAsync(Task task, CancellationToken cancellationToken) {
        _context.Task.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }
    
    public async System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken) {
        var task = await _context.Task.FindAsync(new object[] { id }, cancellationToken);
        _context.Task.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Task>> GetTasksForTomorrow(CancellationToken cancellationToken) {
        DateTimeOffset tomorrowUtc = DateTimeOffset.UtcNow.AddDays(1).Date;
        Console.WriteLine(tomorrowUtc);
        
        return await _context.Task
            .Include(t => t.Worker)
            .Where(t => t.Deadline.Date == tomorrowUtc.Date)
            .OrderBy(t => t.Deadline)
            .ToListAsync(cancellationToken);
    }
    
    public async System.Threading.Tasks.Task UpdateTaskStatusAsync(long taskId, TaskStatus status, CancellationToken cancellationToken) {
        var task = await _context.Task.FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task != null) {
            task.Status = status;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}