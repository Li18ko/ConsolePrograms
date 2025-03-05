namespace WebInstallationOfFloorsApplication;

public class Function {
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public ICollection<RoleFunction> RoleFunctions { get; set; }
}