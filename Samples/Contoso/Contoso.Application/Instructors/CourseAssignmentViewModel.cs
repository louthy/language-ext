using LanguageExt;

namespace Contoso.Application.Instructors
{
    public class CourseAssignmentViewModel : Record<CourseAssignmentViewModel>
    {
        public CourseAssignmentViewModel(int courseId, string title, int credits, int? departmentId)
        {
            CourseId = courseId;
            Title = title;
            Credits = credits;
            DepartmentId = departmentId;
        }

        public int CourseId { get; }
        public string Title { get; }
        public int Credits { get; }
        public int? DepartmentId { get; }
    }
}
