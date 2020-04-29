using Contoso.Core.Domain;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Queries
{
    public class GetCourseById : Record<GetCourseById>, IRequest<Option<CourseViewModel>>
    {
        public GetCourseById(int courseId) => CourseId = courseId;

        public int CourseId { get; }
    }
}
