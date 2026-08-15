namespace smart_table.Models
{
    public class MachineDetail
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int? ProductionYear { get; set; }
        public int OperationHours { get; set; } = 0;
        public decimal DowntimeHours { get; set; } = 0.00m;
        public decimal? Ahs { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int? ActionId { get; set; }
        public decimal? Last7Days { get; set; }
        public decimal? Last30Days { get; set; }
        public decimal? Last90Days { get; set; }
        public DateTime FirstUpdate { get; set; }
        public DateTime LastUpdate { get; set; }

        // --- Computed Properties untuk kalkulasi selisih ---
        public decimal? Diff7Days => (Ahs.HasValue && Last7Days.HasValue) ? Ahs.Value - Last7Days.Value : null;
        public decimal? Diff30Days => (Ahs.HasValue && Last30Days.HasValue) ? Ahs.Value - Last30Days.Value : null;
        public decimal? Diff90Days => (Ahs.HasValue && Last90Days.HasValue) ? Ahs.Value - Last90Days.Value : null;
    }
}