namespace smart_table.Models
{
    public class SmartPrioritizationSummary
    {
        public int Routine { get; set; }
        public int Minor { get; set; }
        public int Major { get; set; }
        public int Critical { get; set; }
        public int TotalMachines { get; set; }
    }
}
