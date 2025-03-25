namespace WebInstallationOfFloorsApplication;

public abstract class BaseFilterDto {
    public string Sort { get; set; } = "Id";
    public string Order { get; set; } = "asc"; 
    public int Skip { get; set; } = 0; 
    public int Take { get; set; } = 10; 
}