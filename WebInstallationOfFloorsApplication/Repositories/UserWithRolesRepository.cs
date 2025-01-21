using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class UserWithRolesRepository {
    private readonly AppDbContext _context;

    public UserWithRolesRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken) {
        return await _context.User
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);

    }
    
    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _context.User
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
    
    public async Task<int?> InsertUserAsync(User user, CancellationToken cancellationToken) {
        await _context.User.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken) {
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