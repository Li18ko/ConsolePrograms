namespace WebInstallationOfFloorsApplication;

public class PagedResult<T> {
    public int Count { get; set; }
    public IEnumerable<T> Items { get; set; }
}