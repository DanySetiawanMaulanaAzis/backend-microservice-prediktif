namespace smart_table.Models
{
    public class SmartPrioritization
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public decimal? Ahs { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? ActionId { get; set; }
        public string? Action { get; set; }
        public bool Has30DaysHistory { get; set; }
        public int? Current30Days { get; set; }
        public int? Last30Days { get; set; }
        public int? Diff30Days { get; set; }
    }
}
