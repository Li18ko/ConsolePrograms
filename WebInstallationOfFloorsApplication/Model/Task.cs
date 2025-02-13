namespace WebInstallationOfFloorsApplication;

public class Task {
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public string Comment { get; set; }
    public string Address{ get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int WorkerId { get; set; }
    public User Worker { get; set; }
    public TaskStatus Status { get; set; }
}

public enum TaskStatus {
    Open,
    Done,
    Reject
}