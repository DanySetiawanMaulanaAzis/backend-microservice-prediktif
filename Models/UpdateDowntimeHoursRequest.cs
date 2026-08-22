namespace smart_table.Models
{
    public class UpdateDowntimeHoursRequest
    {
        public int MachineId { get; set; }
        public int SecondsToAdd { get; set; }
    }
}
