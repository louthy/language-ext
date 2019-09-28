using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Commands
{
    public class CreateCourse : Record<CreateCourse>, IRequest<Either<Error, int>>
    {
        public CreateCourse(string title, int credits, int departmentId)
        {
            Title = title;
            Credits = credits;
            DepartmentId = departmentId;
        }

        public string Title { get; }
        public int Credits { get; }
        public int DepartmentId { get; }
    }
}
