namespace smart_table.Models
{
    public class Machine
    {
        public int Id { get; set; }

        public string MachineName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int ProductionYear { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
