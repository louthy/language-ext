using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Instructors.Commands
{
    public class UpdateInstructor : Record<UpdateInstructor>, IRequest<Either<Error, Task>>
    {
        public UpdateInstructor(int instructorId, string firstName, string lastName)
        {
            InstructorId = instructorId;
            FirstName = firstName;
            LastName = lastName;
        }

        public int InstructorId { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
