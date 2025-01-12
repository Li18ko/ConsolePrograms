using System.Collections.ObjectModel;

namespace TelegramBot.Model;

public class User {
    public int Id {get;set;}
    public string Name {get;set;}
    public string Login {get;set;}
    public string Password {get;set;}
    public ICollection<UserRole> UserRoles { get; set; }
}