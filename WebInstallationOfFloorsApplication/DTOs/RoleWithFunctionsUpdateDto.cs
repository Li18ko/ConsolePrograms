namespace WebInstallationOfFloorsApplication;

public class RoleWithFunctionsUpdateDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public List<int> FunctionIds { get; set; }  
}