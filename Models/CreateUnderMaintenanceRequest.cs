namespace smart_table.Models
{
    public class CreateUnderMaintenanceRequest
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public bool Maintenance { get; set; } = true;
        public int EventId { get; set; }
    }
}
