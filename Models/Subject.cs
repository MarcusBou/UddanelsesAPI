namespace UddanelsesAPI.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid GUID { get; set; } = Guid.NewGuid();
        public IEnumerable<Module> Modules { get; set; }
        
    }
}
