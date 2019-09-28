namespace Contoso.Core.Domain
{
    public class CourseAssignment
    {
        public int CourseAssignmentId { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public Instructor Instructor { get; set; }
        public Course Course { get; set; }
    }
}
