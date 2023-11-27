namespace UddanelsesAPI.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid GUID { get; set; } = Guid.NewGuid();
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Type { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }
}
