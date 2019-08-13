using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Commands
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourse, Either<Error, Task>>
    {
        private ICourseRepository _courseRepository;
        public DeleteCourseHandler(ICourseRepository courseRepository) => _courseRepository = courseRepository;

        public async Task<Either<Error, Task>> Handle(DeleteCourse request, CancellationToken cancellationToken) =>
            (await CourseMustExist(request))
                .Map(DoDeletion)
                .ToEither<Task>();

        private Task DoDeletion(int courseId) => _courseRepository.Delete(courseId);

        private async Task<Validation<Error, int>> CourseMustExist(DeleteCourse course) =>
            (await _courseRepository.Get(course.CourseId))
                .ToValidation<Error>($"Course Id: {course.CourseId} does not exist.")
                .Map(c => c.CourseId);
    }
}
