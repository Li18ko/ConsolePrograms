namespace WebInstallationOfFloorsApplication;

public class RoleFilterDto: BaseFilterDto {
    public IEnumerable<bool>? Status { get; set; } = null;
}