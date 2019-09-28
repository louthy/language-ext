using System;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Instructors.Commands
{
    public class CreateInstructor : Record<CreateInstructor>, IRequest<Either<Error, int>>
    {
        public CreateInstructor(string firstName, string lastName, DateTime hireDate)
        {
            FirstName = firstName;
            LastName = lastName;
            HireDate = hireDate;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public DateTime HireDate { get; }
    }
}
