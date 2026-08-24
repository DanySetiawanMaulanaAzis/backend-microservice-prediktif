namespace smart_table.Models
{
    public class UnderMaintenance
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public bool Maintenance { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? MachineDetailId { get; set; } // Property baru
        public string Location { get; set; }
        public decimal? Ahs { get; set; }
        public int? EventId { get; set; }
        public string? Event { get; set; }
        public string? MaintenanceType { get; set; }
        public int? ProductionYear { get; set; }
        public int OperationHours { get; set; } = 0;
        public decimal DowntimeHours { get; set; } = 0.00m;
        public string StatusName { get; set; } = string.Empty;
        public int? DaysSinceLastService { get; set; }
        public int? DaysBetweenEvents { get; set; }
    }
}
