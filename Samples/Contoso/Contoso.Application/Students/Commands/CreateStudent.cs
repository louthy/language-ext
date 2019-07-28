using System;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Commands
{
    public class CreateStudent : IRequest<Either<Error, int>>
    {
        public CreateStudent()
        { }

        public CreateStudent(string firstName, string lastName, DateTime enrollmentDate)
        {
            FirstName = firstName;
            LastName = lastName;
            EnrollmentDate = enrollmentDate;
        }

        public string FirstName { get; set;  }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
