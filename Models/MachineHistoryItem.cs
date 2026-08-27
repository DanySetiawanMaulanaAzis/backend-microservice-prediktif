namespace smart_table.Models
{
    // A single undermaintenance record for a machine, regardless of whether it's
    // still active (maintenance = 1) or completed - unlike GetUnderMaintenanceAsync,
    // which only surfaces currently-active records.
    public class MachineHistoryItem
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public bool Maintenance { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? EventId { get; set; }
        public string? Event { get; set; }
        public string? MaintenanceType { get; set; }
        public int? ActionId { get; set; }
        public string? ActionBy { get; set; }
        public string? ActionTaken { get; set; }
        public DateTime? ActionCreatedAt { get; set; }
    }
}
