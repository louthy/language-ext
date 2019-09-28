using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Queries
{
    public class GetCourseByIdHandler : IRequestHandler<GetCourseById, Option<CourseViewModel>>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseByIdHandler(ICourseRepository courseRepository) => 
            _courseRepository = courseRepository;

        public Task<Option<CourseViewModel>> Handle(GetCourseById request, CancellationToken cancellationToken) => 
            Fetch(request.CourseId)
                .MapT(Project);

        private Task<Option<Course>> Fetch(int id) => _courseRepository.Get(id);

        private static CourseViewModel Project(Course course) => 
            new CourseViewModel(course.CourseId, course.Title, course.Credits, course.DepartmentId);
    }
}
