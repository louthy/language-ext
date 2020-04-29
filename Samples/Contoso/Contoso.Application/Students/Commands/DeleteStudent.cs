using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Commands
{
    public class DeleteStudent : Record<DeleteStudent>, IRequest<Either<Error, Task>>
    {
        public DeleteStudent(int studentId) => StudentId = studentId;

        public int StudentId { get; }
    }
}
