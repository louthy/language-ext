using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Commands
{
    public class CreateCourseAssignment : Record<CreateCourseAssignment>, IRequest<Validation<Error, int>>
    {
        public CreateCourseAssignment(int courseId, int instructorId)
        {
            CourseId = courseId;
            InstructorId = instructorId;
        }

        public int CourseId { get; }
        public int InstructorId { get; }
    }
}
