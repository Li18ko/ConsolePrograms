namespace WebInstallationOfFloorsApplication;

public class Role {
    public int Id { get; set; }
    public string Name { get; set; } 
    public string Description { get; set; } 
    public bool Active { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleFunction> RoleFunctions { get; set; }
}