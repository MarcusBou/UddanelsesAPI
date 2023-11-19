namespace UddanelsesAPI.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
        public Subject Subject { get; set; }
    }
}
