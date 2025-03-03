using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class UserWithRolesRepository {
    private readonly AppDbContext _context;

    public UserWithRolesRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(string sort, CancellationToken cancellationToken) {
        var query = _context.User
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsQueryable();

        query = sort switch {
            "nameAsc" => query.OrderBy(u => u.Name),
            "nameDesc" => query.OrderByDescending(u => u.Name),
            "emailAsc" => query.OrderBy(u => u.Email),
            "emailDesc" => query.OrderByDescending(u => u.Email),
            "createdAtAsc" => query.OrderBy(u => u.CreatedAt),
            "createdAtDesc" => query.OrderByDescending(u => u.CreatedAt),
            "lastRevisionAsc" => query.OrderBy(u => u.LastRevision),
            "lastRevisionDesc" => query.OrderByDescending(u => u.LastRevision),
            _ => query.OrderBy(u => u.Name)
        };
        
        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _context.User
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
    
    public async Task<bool> IsLoginTakenByOtherUserAsync(int id, string login, CancellationToken cancellationToken) {
        return await _context.User
            .AnyAsync(u => u.Login == login && u.Id != id, cancellationToken);
    }
    
    public async Task<bool> IsLoginTakenAsync(string login, CancellationToken cancellationToken) {
        return await _context.User
            .AnyAsync(u => u.Login == login, cancellationToken);
    }
    
    public async Task<bool> IsEmailTakenByOtherUserAsync(int id, string email, CancellationToken cancellationToken) {
        return await _context.User
            .AnyAsync(u => u.Email == email && u.Id != id, cancellationToken);
    }
    
    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken) {
        return await _context.User
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    
    public async Task<string> GetUserPasswordAsync(int id, CancellationToken cancellationToken) {
        return await _context.User
            .Where(u => u.Id == id)
            .Select(u => u.Password)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<int?> InsertUserAsync(User? user, CancellationToken cancellationToken) {
        await _context.User.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    public async Task<User?> UpdateUserAsync(User? user, CancellationToken cancellationToken) {
        _context.User.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
    
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        var user = await _context.User.FindAsync(new object[] { id }, cancellationToken);
        _context.User.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}