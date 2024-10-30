using System.Text.Json.Serialization;

namespace GitHubApiClient {
    public record class RepositoryOwner(
        [property: JsonPropertyName("login")] string Login);
}