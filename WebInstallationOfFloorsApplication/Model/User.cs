namespace WebInstallationOfFloorsApplication;

public class User {
    public int Id {get;set;}
    public string Name {get;set;}
    public string Email {get;set;}
    public string Login {get;set;}
    public string Password {get;set;}
    public long ChatId {get;set;}
    public DateTimeOffset CreatedAt {get;set;}
    public DateTimeOffset LastRevision {get;set;}
    public ICollection<UserRole> UserRoles { get; set; }
}