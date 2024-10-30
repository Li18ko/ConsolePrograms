using System.Text.Json.Serialization;

namespace GitHubApiClient {
    public record class Repository(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("html_url")] Uri GitHubHomeUrl,
        [property: JsonPropertyName("owner")] RepositoryOwner Owner);
}