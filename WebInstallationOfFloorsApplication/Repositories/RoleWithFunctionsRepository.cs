using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class RoleWithFunctionsRepository {
    private readonly AppDbContext _context;
    public RoleWithFunctionsRepository(AppDbContext context) {
        _context = context;
    }
    
    public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken) {
        return await _context.Role
            .Include(rf => rf.RoleFunctions) 
            .ThenInclude(f => f.Function) 
            .ToListAsync(cancellationToken);
    }

    public async Task<Role> GetRoleAsync(int id, CancellationToken cancellationToken) {
        return await _context.Role
            .Include(rf => rf.RoleFunctions)
            .ThenInclude(f => f.Function)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
    
    public async Task<int?> InsertRoleAsync(Role role, CancellationToken cancellationToken) {
        await _context.Role.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return role.Id;
    }
    
    public async Task<Role?> UpdateRoleAsync(Role? role, CancellationToken cancellationToken) {
        _context.Role.Update(role);
        await _context.SaveChangesAsync(cancellationToken);
        return role;
    }
    
    public async System.Threading.Tasks.Task DeleteRoleAsync(int id, CancellationToken cancellationToken) {
        var role = await _context.Role.FindAsync(new object[] { id }, cancellationToken);
        _context.Role.Remove(role); 
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> IsRoleInUseAsync(int id, CancellationToken cancellationToken) {
        return await _context.UserRole
            .AnyAsync(ur => ur.RoleId == id, cancellationToken); 
    }
    
}