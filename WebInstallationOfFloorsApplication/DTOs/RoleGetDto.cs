namespace WebInstallationOfFloorsApplication;

public class RoleGetDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public List<FunctionDto> Functions { get; set; }
}

public class FunctionDto {
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}