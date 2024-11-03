using System.Text.Json.Serialization;

namespace GitHubApiClient {
    public record class CommitInfo(
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("author")] CommitAuthor Author);
}