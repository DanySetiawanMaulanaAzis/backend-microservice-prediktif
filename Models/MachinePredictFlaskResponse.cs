using System.Text.Json.Serialization;

namespace smart_table.Models
{
    public class MachinePredictFlaskResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; } = string.Empty;

        [JsonPropertyName("health_score")]
        public double HealthScore { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
