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
using Unit = LanguageExt.Unit;

namespace Contoso.Application.Courses.Commands
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourse, Validation<Error, Task<Unit>>>
    {
        private readonly IMediator _mediator;
        private readonly ICourseRepository _courseRepository;

        public UpdateCourseHandler(IMediator mediator, ICourseRepository courseRepository)
        {
            _mediator = mediator;
            _courseRepository = courseRepository;
        }

        public Task<Validation<Error, Task<Unit>>> Handle(UpdateCourse request, CancellationToken cancellationToken) =>
            Validate(request)
                .MapT(c => ApplyUpdate(c, request))
                .MapT(Persist);

        private Course ApplyUpdate(Course c, UpdateCourse request) => 
            new Course
            {
                CourseId = c.CourseId,
                DepartmentId = request.DepartmentId,
                Credits = request.Credits,
                Title = request.Title
            };

        private Task<Unit> Persist(Course c) => _courseRepository.Update(c);

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
