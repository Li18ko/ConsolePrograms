namespace WebInstallationOfFloorsApplication;

public class TaskUpdateDto {
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Deadline { get; set; }
    public string Comment { get; set; }
    public string Address { get; set; }
    public int WorkerId { get; set; }
    public TaskStatus Status { get; set; }
}