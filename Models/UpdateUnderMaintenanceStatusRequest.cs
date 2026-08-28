namespace smart_table.Models
{
    public class UpdateUnderMaintenanceStatusRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public double Ahs { get; set; }
        public int StatusId { get; set; }
    }
}
