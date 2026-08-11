namespace smart_table.Models
{
    public class CreateandUpdateMachineRequest
    {
        public string MachineName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
    }
}
