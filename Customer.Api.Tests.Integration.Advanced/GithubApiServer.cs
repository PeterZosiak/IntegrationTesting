using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Customer.Api.Tests.Integration.Advanced
{
    public class GithubApiServer : IDisposable
    {
        private WireMockServer _wireMockServer;

        public string Url => _wireMockServer.Url!;

        public void Start() {
            _wireMockServer = WireMockServer.Start();
        }

        public void SetupUser(string username)
        {
            _wireMockServer.Given(Request.Create().WithPath($"/users/{username}").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBodyAsJson(GenerateGithubUser(username))
                .WithHeader("Content-Type", "application/json")
                );
        }

        private static string GenerateGithubUser(string username)
        {
            return $@"
                                    {{""login"": ""{username}"",
                                      ""id"": 20397916,
                                      ""node_id"": ""MDQ6VXNlcjIwMzk3OTE2"",
                                      ""avatar_url"": ""https://avatars.githubusercontent.com/u/20397916?v=4"",
                                      ""gravatar_id"": """",
                                      ""url"": ""https://api.github.com/users/{username}"",
                                      ""html_url"": ""https://github.com/{username}"",
                                      ""followers_url"": ""https://api.github.com/users/{username}/followers"",
                                      ""following_url"": ""https://api.github.com/users/{username}/following{{/other_user}}"",
                                      ""gists_url"": ""https://api.github.com/users/{username}/gists{{/gist_id}}"",
                                      ""starred_url"": ""https://api.github.com/users/{username}/starred{{/owner}}{{/repo}}"",
                                      ""subscriptions_url"": ""https://api.github.com/users/{username}/subscriptions"",
                                      ""organizations_url"": ""https://api.github.com/users/{username}/orgs"",
                                      ""repos_url"": ""https://api.github.com/users/{username}/repos"",
                                      ""events_url"": ""https://api.github.com/users/{username}/events{{/privacy}}"",
                                      ""received_events_url"": ""https://api.github.com/users/{username}/received_events"",
                                      ""type"": ""User"",
                                      ""site_admin"": false,
                                      ""name"": ""{username}"",
                                      ""company"": null,
                                      ""location"": ""Prague"",
                                      ""email"": null,
                                      ""hireable"": null,
                                      ""bio"": ""Nothing important here"",
                                      ""twitter_username"": null,
                                      ""public_repos"": 2,
                                      ""public_gists"": 0,
                                      ""followers"": 0,
                                      ""following"": 0,
                                      ""created_at"": ""2016 - 07 - 11T11: 30:45Z"",
                                      ""updated_at"": ""2023 - 08 - 02T06: 12:38Z""
                                    }}
                                    ";
        }

        public void Dispose()
        {
            _wireMockServer.Stop();
            _wireMockServer.Dispose();
        }
    }
}
