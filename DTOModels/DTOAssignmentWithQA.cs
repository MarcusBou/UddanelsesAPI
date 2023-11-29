using DTOModels;

namespace UddanelsesAPI.DTOModels
{
    public class DTOAssignmentWithQA : DTOAssignment
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string? Video { get; set; }
        public string Type { get; set; }
        public Guid? NextAssignmentId { get; set; }
    }
}
