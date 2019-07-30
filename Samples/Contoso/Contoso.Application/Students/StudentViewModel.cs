using System;
using LanguageExt;

namespace Contoso.Application.Students
{
    public class StudentViewModel : Record<StudentViewModel>
    {
        public StudentViewModel(int studentId,
            string firstName, 
            string lastName, 
            DateTime enrollmentDate, 
            Lst<StudentEnrollmentViewModel> enrollments)
        {
            StudentId = studentId;
            FirstName = firstName;
            LastName = lastName;
            EnrollmentDate = enrollmentDate;
            Enrollments = enrollments;
        }

        public int StudentId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName => LastName + ", " + FirstName;
        public DateTime EnrollmentDate { get; }

        public Lst<StudentEnrollmentViewModel> Enrollments { get; }
    }
}
