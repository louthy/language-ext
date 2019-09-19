using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;
using static Contoso.Validators;
using Contoso.Application.Departments.Queries;

namespace Contoso.Application.Courses.Commands
{
    public class CreateCourseHandler : IRequestHandler<CreateCourse, Either<Error, int>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMediator _mediator;

        public CreateCourseHandler(ICourseRepository courseRepository,
            IMediator mediator)
        {
            _courseRepository = courseRepository;
            _mediator = mediator;
        }

        public Task<Either<Error, int>> Handle(CreateCourse request, CancellationToken cancellationToken) =>
            Validate(request)
                .MapT(PersistCourse)
                .Bind(v => v.ToEitherAsync());

        private Task<int> PersistCourse(Course c) => _courseRepository.Add(c);

        private async Task<Validation<Error, Course>> Validate(CreateCourse create) => 
            (await DepartmentMustExist(create), ValidateTitle(create))
                .Apply((id, title) => new Course { Title = title, DepartmentId = id, Credits = create.Credits });

        private Validation<Error, string> ValidateTitle(CreateCourse course) =>
            NotEmpty(course.Title)
                .Bind(NotLongerThan(50));

        private async Task<Validation<Error, int>> DepartmentMustExist(CreateCourse create) => 
            (await _mediator.Send(new GetDepartmentById(create.DepartmentId)))
                .ToValidation<Error>($"Department Id {create.DepartmentId} does not exist.")
                .Map(v => v.DepartmentId);
    }
}
