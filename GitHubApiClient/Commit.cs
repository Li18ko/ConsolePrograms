using System.Text.Json.Serialization;

namespace GitHubApiClient {
    public record class Commit(
        [property: JsonPropertyName("sha")] string Sha,
        [property: JsonPropertyName("commit")] CommitInfo CommitInfo)
    {
        public string Message() {
            return CommitInfo.Message;
        }
        public string AuthorName() {
            return CommitInfo.Author?.Name ?? "Unknown";
        }

        public DateTime Date() {
            return CommitInfo.Author?.Date ?? DateTime.MinValue;
        }
    }
}