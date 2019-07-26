namespace Contoso.Core.Domain
{
    public class OfficeAssignment
    {
        public int OfficeAssignmentId { get; set; }
        public int InstructorId { get; set; }
        public string Location { get; set; }
        public Instructor Instructor { get; set; }
    }
}
