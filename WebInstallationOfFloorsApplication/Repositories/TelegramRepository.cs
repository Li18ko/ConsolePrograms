using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class TelegramRepository {
    private readonly AppDbContext _context;
    
    public TelegramRepository(AppDbContext context) {
        _context = context;
    }
    
    public async Task<IEnumerable<Task>> GetPendingTasksAsync(CancellationToken cancellationToken) {
        DateTime tomorrowUtc = DateTime.UtcNow.AddHours(3).AddDays(1).Date;
        Console.WriteLine(tomorrowUtc);
        
        return await _context.Task
            .Include(t => t.Worker)
            .Where(t => t.Status == TaskStatus.Open && t.Deadline.ToUniversalTime().Date == tomorrowUtc)
            .OrderBy(t => t.Deadline)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Task> GetTaskAsync(int id, CancellationToken cancellationToken) {
        return await _context.Task.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    
    public async System.Threading.Tasks.Task UpdateTaskStatusAsync(long taskId, TaskStatus status, CancellationToken cancellationToken) {
        var task = await _context.Task.FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task != null) {
            task.Status = status;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

}