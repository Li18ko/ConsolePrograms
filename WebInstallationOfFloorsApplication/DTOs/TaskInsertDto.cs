namespace WebInstallationOfFloorsApplication;

public class TaskInsertDto {
    public string Title { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public string Comment { get; set; }
    public string Address { get; set; }
    public int WorkerId { get; set; }
}
