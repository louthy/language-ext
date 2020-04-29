using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Instructors.Commands
{
    public class DeleteInstructor : Record<DeleteInstructor>, IRequest<Either<Error, Task>>
    {
        public DeleteInstructor(int instructorId) => InstructorId = instructorId;

        public int InstructorId { get; }
    }
}
