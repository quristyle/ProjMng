using System.Text.Json.Serialization;

namespace ProjMngWasm.Models.GitHub
{
    public class User
    {
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }
    }
}