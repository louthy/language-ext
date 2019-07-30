using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Application.Students
{
    public class StudentEnrollmentViewModel : Record<StudentEnrollmentViewModel>
    {
        public StudentEnrollmentViewModel(int courseId, string title, int credits, int? departmentId, Grade? grade)
        {
            CourseId = courseId;
            Title = title;
            Credits = credits;
            DepartmentId = departmentId;
            Grade = grade;
        }

        public int CourseId { get; }
        public string Title { get; }
        public int Credits { get; }
        public int? DepartmentId { get; }
        public Grade? Grade { get; }
    }
}
