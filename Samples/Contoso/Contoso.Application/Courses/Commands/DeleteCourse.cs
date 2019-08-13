using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Commands
{
    public class DeleteCourse : Record<DeleteCourse>, IRequest<Either<Error, Task>>
    {
        public DeleteCourse(int courseId) => CourseId = courseId;
        public int CourseId { get; }
    }
}
