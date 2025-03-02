using System.ComponentModel.DataAnnotations;

namespace WebInstallationOfFloorsApplication;

public class UserWithRolesUpdateDto {
    public int Id {get;set;}
    
    [Required(ErrorMessage = "Имя обязательно")]
    public string Name {get;set;}
    
    [Required(ErrorMessage = "Почта обязательна")]
    [EmailAddress(ErrorMessage = "Некорректный формат электронной почты")]
    public string Email {get;set;}
    
    [Required(ErrorMessage = "Логин обязателен")]
    public string Login {get;set;}
    
    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "ID чата обязательно")]
    public long ChatId {get;set;}
    public List<int> RoleIds { get; set; }
}