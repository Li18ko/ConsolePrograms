using System.ComponentModel.DataAnnotations;

namespace WebInstallationOfFloorsApplication;

public class UserUpdateDto {
    public int Id {get;set;}
    
    [Required(ErrorMessage = "Имя обязательно")]
    public string Name {get;set;}
    
    [Required(ErrorMessage = "Почта обязательна")]
    [EmailAddress(ErrorMessage = "Некорректный формат электронной почты")]
    public string Email {get;set;}
    
    [Required(ErrorMessage = "Логин обязателен")]
    public string Login {get;set;}
    
    private string _password;
    
    public string Password {
        get => _password;
        set {
            if (!string.IsNullOrEmpty(value) && value.Length < 6) {
                throw new ArgumentException("Пароль должен содержать минимум 6 символов");
            }
            _password = value;
        }
    }
    
    [Required(ErrorMessage = "ID чата обязательно")]
    [TelegramChatId]
    public long ChatId {get;set;}
    public List<int> RoleIds { get; set; }
}