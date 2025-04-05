namespace WebInstallationOfFloorsApplication;

public class UserFilterDto: BaseFilterDto {
    public new string Sort { get; set; } = "LastRevision"; 
    public new string Order { get; set; } = "desc";
    public IEnumerable<int>? Role { get; set; } = null;
    public string? Search { get; set; } = "";
}