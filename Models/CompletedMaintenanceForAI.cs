namespace smart_table.Models
{
    public class CompletedMaintenanceForAI
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public int? ProductionYear { get; set; }
        public int? MachineAge { get; set; }
        public int OperationHours { get; set; } = 0;
        public decimal DowntimeHours { get; set; } = 0.00m;
        public int? DaysSinceLastService { get; set; }
        public int? DaysBetweenEvents { get; set; }
    }
}
