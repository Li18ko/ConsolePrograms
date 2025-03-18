namespace WebInstallationOfFloorsApplication;

public class UserGetDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public long ChatId { get; set; }
    public DateTimeOffset CreatedAt {get;set;}
    public DateTimeOffset LastRevision {get;set;}
    public List<string> Roles { get; set; }
}