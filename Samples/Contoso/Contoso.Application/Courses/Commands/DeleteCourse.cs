using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Contoso.Application.Courses.Commands
{
    public class DeleteCourse : Record<DeleteCourse>, IRequest<Either<Error, Task<Unit>>>
    {
        public DeleteCourse(int courseId) => CourseId = courseId;
        public int CourseId { get; }
    }
}
