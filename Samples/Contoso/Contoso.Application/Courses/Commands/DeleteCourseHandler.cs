using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Contoso.Application.Courses.Commands
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourse, Either<Error, Task<Unit>>>
    {
        private ICourseRepository _courseRepository;
        public DeleteCourseHandler(ICourseRepository courseRepository) => _courseRepository = courseRepository;

        public Task<Either<Error, Task<Unit>>> Handle(DeleteCourse request, CancellationToken cancellationToken) =>
            CourseMustExist(request)
                .MapT(DoDeletion)
                .ToEither();

        private Task<Unit> DoDeletion(int courseId) => _courseRepository.Delete(courseId);

        private async Task<Validation<Error, int>> CourseMustExist(DeleteCourse course) =>
            (await _courseRepository.Get(course.CourseId))
                .ToValidation<Error>($"Course Id: {course.CourseId} does not exist.")
                .Map(c => c.CourseId);
    }
}
