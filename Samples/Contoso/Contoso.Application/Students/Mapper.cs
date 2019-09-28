using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Application.Students
{
    internal class Mapper
    {
        internal static StudentViewModel ProjectToViewModel(Student student) =>
            new StudentViewModel(student.StudentId, student.FirstName, student.LastName,
                student.EnrollmentDate, new Lst<StudentEnrollmentViewModel>(student.Enrollments.Map(c => Project(c.Course, c.Grade))));

        private static StudentEnrollmentViewModel Project(Course course, Grade? grade) =>
            new StudentEnrollmentViewModel(course.CourseId, course.Title, course.Credits, course.DepartmentId, grade);

    }
}
