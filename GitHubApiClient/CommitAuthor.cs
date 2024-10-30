using System.Text.Json.Serialization;

namespace GitHubApiClient {
    public record class CommitAuthor(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("date")] DateTime Date);
}