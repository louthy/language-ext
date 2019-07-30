using System;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Application.Instructors
{
    public class InstructorViewModel : Record<InstructorViewModel>
    {
        public InstructorViewModel(int instructorId, string firstName, string lastName, DateTime hireDate, Lst<CourseAssignmentViewModel> courses)
        {
            InstructorId = instructorId;
            FirstName = firstName;
            LastName = lastName;
            HireDate = hireDate;
            Courses = courses;
        }

        public int InstructorId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName => LastName + ", " + FirstName;
        public DateTime HireDate { get; }
        public Lst<CourseAssignmentViewModel> Courses { get; }
    }
}
