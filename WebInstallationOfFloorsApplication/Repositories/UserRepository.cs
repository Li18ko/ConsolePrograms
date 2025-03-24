using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class UserRepository {
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsersAsync(string sort, string order, 
        IEnumerable<int>? filter, string search, int skip, int take, CancellationToken cancellationToken) {
        var query = _context.User
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsQueryable();
        
        if (filter != null && filter.Any()) {
            query = query.Where(u => u.UserRoles.Any(ur => filter.Contains(ur.Role.Id)));
        }
        
        if (!string.IsNullOrEmpty(search)) {
            query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search) || u.Login.Contains(search));
        }
        
        if (!string.IsNullOrEmpty(sort)) {
            var parameter = Expression.Parameter(typeof(User), "u");
            var property = Expression.Property(parameter, sort);    
            var lambda = Expression.Lambda(property, parameter);

            var methodName = order?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(User), property.Type },
                query.Expression,
                Expression.Quote(lambda)
            );

            query = query.Provider.CreateQuery<User>(resultExpression);
        }
        else {
            query = query.OrderByDescending(u => u.LastRevision);
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var users = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        
        return (users, totalCount);
    }
    
    public async Task<IEnumerable<User>> GetAllUsersWithoutSortingAsync(CancellationToken cancellationToken) {
        var users = await _context.User
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);
        
        return users;
    }
    
    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _context.User
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
    
    
    public async Task<bool> IsLoginTakenAsync(int? id, string login, CancellationToken cancellationToken) {
        return await _context.User
            .AnyAsync(u => u.Login == login && (!id.HasValue || u.Id != id.Value), cancellationToken);
    }
    
    public async Task<bool> IsEmailTakenAsync(int? id, string email, CancellationToken cancellationToken) {
        return await _context.User
            .AnyAsync(u => u.Email == email && (!id.HasValue || u.Id != id.Value), cancellationToken);
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