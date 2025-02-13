namespace WebInstallationOfFloorsApplication;

public class UserWithRolesGetDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public long ChatId { get; set; }
    public DateTimeOffset CreatedAt {get;set;}
    public DateTimeOffset LastRevision {get;set;}
    public List<int> Roles { get; set; }
}