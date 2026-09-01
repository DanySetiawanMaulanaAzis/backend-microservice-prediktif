namespace smart_table.Models
{
    public class MachinePredictResponse
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public double HealthScore { get; set; }
    }
}
