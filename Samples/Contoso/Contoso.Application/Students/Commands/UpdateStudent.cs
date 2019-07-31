using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Commands
{
    public class UpdateStudent : Record<UpdateStudent>, IRequest<Either<Error, Task>>
    {
        public UpdateStudent(int studentId, string firstName, string lastName)
        {
            StudentId = studentId;
            FirstName = firstName;
            LastName = lastName;
        }

        public int StudentId { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
