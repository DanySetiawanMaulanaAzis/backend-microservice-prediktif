namespace smart_table.Models
{
    public class CompletedMaintenance
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public bool Maintenance { get; set; }
        public DateTime LastUpdate { get; set; }
        public int? MachineDetailId { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal? Ahs { get; set; }
        public int? EventId { get; set; }
        public string? Event { get; set; }
        public string? MaintenanceType { get; set; }
        public int? ActionId { get; set; }       
        public string? Action { get; set; }
    }
}
