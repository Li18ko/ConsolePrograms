using System.ComponentModel.DataAnnotations;

namespace WebInstallationOfFloorsApplication;

public class RoleUpdateDto {
    public int Id { get; set; }
    [Required(ErrorMessage = "Название обязательно")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Описание обязательно")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Активность роли обязательна")]
    public bool Active { get; set; }
    public List<int> FunctionIds { get; set; }  
}