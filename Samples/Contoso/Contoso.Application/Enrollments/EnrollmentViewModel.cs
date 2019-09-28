using System;
using Contoso.Core.Domain;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso.Application.Enrollments
{
    public class EnrollmentViewModel : Record<EnrollmentViewModel>
    {
        public EnrollmentViewModel(int enrollmentId, int courseId, string courseName, 
            int studentId, string studentName, Grade? grade)
        {
            EnrollmentId = enrollmentId;
            CourseId = courseId;
            CourseName = courseName;
            StudentId = studentId;
            StudentName = studentName;
            Grade = Optional(grade)
                .Map(g => Enum.GetName(typeof(Grade), g))
                .IfNone("");
        }

        public int EnrollmentId { get; }
        public int CourseId { get; }
        public string CourseName { get; }
        public int StudentId { get; }
        public string StudentName { get; }
        public string Grade { get; }
    }
}
