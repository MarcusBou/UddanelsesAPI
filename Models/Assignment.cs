namespace UddanelsesAPI.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }
}
