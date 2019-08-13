using System.Threading;
using System.Threading.Tasks;
using Contoso.Application.Departments.Queries;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Application.Courses.Commands
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourse, Either<Error, Task>>
    {
        private readonly IMediator _mediator;
        private readonly ICourseRepository _courseRepository;

        public UpdateCourseHandler(IMediator mediator, ICourseRepository courseRepository)
        {
            _mediator = mediator;
            _courseRepository = courseRepository;
        }

        public async Task<Either<Error, Task>> Handle(UpdateCourse request, CancellationToken cancellationToken) =>
            (await Validate(request))
                .Map(c => ApplyUpdate(c, request))
                .ToEither<Task>();

        private Task ApplyUpdate(Course c, UpdateCourse request)
        {
            c.Title = request.Title;
            c.Credits = request.Credits;
            c.DepartmentId = request.DepartmentId;
            return _courseRepository.Update(c);
        }

        private async Task<Validation<Error, Course>> Validate(UpdateCourse updateCourse) => 
            (ValidateTitle(updateCourse), await DepartmentMustExist(updateCourse),
                await CourseMustExist(updateCourse)).Apply((t, d, course) => course);

        private Validation<Error, string> ValidateTitle(UpdateCourse course) =>
            NotEmpty(course.Title)
                .Bind(NotLongerThan(50));

        private async Task<Validation<Error, Course>> CourseMustExist(UpdateCourse updateCourse) =>
            (await _courseRepository.Get(updateCourse.CourseId))
                .ToValidation<Error>($"Course Id {updateCourse.CourseId} does not exist.");

        private async Task<Validation<Error, int>> DepartmentMustExist(UpdateCourse updateCourse) =>
            (await _mediator.Send(new GetDepartmentById(updateCourse.DepartmentId)))
                .ToValidation<Error>($"Department Id: {updateCourse.DepartmentId} does not exist")
                .Map(d => d.DepartmentId);
    }
}
