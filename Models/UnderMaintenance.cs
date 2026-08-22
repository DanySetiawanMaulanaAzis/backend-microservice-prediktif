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
    }
}
