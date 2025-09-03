using System.Text.Json.Serialization;

namespace API.DTOs
{
    public class GoogleInfo
    {
        public class GoogleAuthRequest
        {
            public required string Code { get; set; }

            [JsonPropertyName("client_id")]
            public required string ClientId { get; set; }

            [JsonPropertyName("client_secret")]
            public required string ClientSecret { get; set; }

            [JsonPropertyName("redirect_uri")]
            public required string RedirectUri { get; set; }
            [JsonPropertyName("grant_type")]
            public required string GrantType { get; set; }
        }
        //public class GoogleTokenResponse
        //{
        //    [JsonPropertyName("access_token")]
        //    public string AccessToken { get; set; } = "";
        //}

        public class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
            [JsonPropertyName("id_token")] public string? IdToken { get; set; }
            [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
            [JsonPropertyName("token_type")] public string? TokenType { get; set; }
            [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
            [JsonPropertyName("scope")] public string? Scope { get; set; }

        }

        public class GoogleUserInfo
        {
            [JsonPropertyName("sub")] public string? Sub { get; set; }
            [JsonPropertyName("email")] public string? Email { get; set; }
            [JsonPropertyName("email_verified")] public bool EmailVerified { get; set; }
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("picture")] public string? Picture { get; set; }
        }
    }
}
