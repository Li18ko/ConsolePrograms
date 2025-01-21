namespace WebInstallationOfFloorsApplication;

public class UserWithRolesUpdateDto {
    public int Id {get;set;}
    public string Name {get;set;}
    public string Login {get;set;}
    public string Password { get; set; }
    public long ChatId {get;set;}
    public List<int> RoleIds { get; set; }
}