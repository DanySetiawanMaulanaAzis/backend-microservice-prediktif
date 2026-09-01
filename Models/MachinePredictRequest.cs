using System.Text.Json.Serialization;

namespace smart_table.Models
{
    public class MachinePredictRequest
    {
        [JsonPropertyName("machine_age")]
        public double MachineAge { get; set; }

        [JsonPropertyName("operating_seconds")]
        public double OperatingSeconds { get; set; }

        [JsonPropertyName("downtime_seconds")]
        public double DowntimeSeconds { get; set; }

        [JsonPropertyName("days_since_last_service")]
        public double DaysSinceLastService { get; set; }

        [JsonPropertyName("days_between_events")]
        public double DaysBetweenEvents { get; set; }
    }
}
