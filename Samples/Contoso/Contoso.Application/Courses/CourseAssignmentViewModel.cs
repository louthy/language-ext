using LanguageExt;

namespace Contoso.Application.Courses
{
    public class CourseAssignmentViewModel : Record<CourseAssignmentViewModel>
    {
        public CourseAssignmentViewModel(int courseAssignmentId, int instructorID, int courseID, string instructorName, string courseName)
        {
            CourseAssignmentId = courseAssignmentId;
            InstructorID = instructorID;
            CourseID = courseID;
            InstructorName = instructorName;
            CourseName = courseName;
        }

        public int CourseAssignmentId { get; }
        public int InstructorID { get; }
        public int CourseID { get; }
        public string InstructorName { get; }
        public string CourseName { get; }
    }
}
