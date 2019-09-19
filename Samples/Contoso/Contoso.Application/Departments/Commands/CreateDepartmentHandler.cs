using System;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Application.Instructors.Queries;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Application.Departments.Commands
{
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartment, Either<Error, int>>
    {

        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMediator _mediator;

        public CreateDepartmentHandler(IDepartmentRepository departmentRepository,
            IMediator mediator)
        {
            _departmentRepository = departmentRepository;
            _mediator = mediator;
        }

        public Task<Either<Error, int>> Handle(CreateDepartment request, CancellationToken cancellationToken) =>
            Validate(request)
                .MapT(PersistDepartment)
                .Bind(v => v.ToEitherAsync());

        private Task<int> PersistDepartment(Department d) => _departmentRepository.Add(d);

        private async Task<Validation<Error, Department>> Validate(CreateDepartment create) => 
            (ValidateDepartmentName(create), ValidateBudget(create), 
                MustStartInFuture(create), await InstructorIdMustExist(create))
                .Apply((n, b, s, i) => new Department { Name = n, Budget = b, StartDate = s, AdministratorId = i });

        private async Task<Validation<Error, int>> InstructorIdMustExist(CreateDepartment createDepartment) =>
            (await _mediator.Send(new GetInstructorById(createDepartment.AdministratorId)))
                .ToValidation<Error>($"Administrator Id {createDepartment.AdministratorId} does not exist")
                .Map(i => i.InstructorId);

        private static Validation<Error, DateTime> MustStartInFuture(CreateDepartment createDepartment) =>
            createDepartment.StartDate > DateTime.UtcNow
                ? Success<Error, DateTime>(createDepartment.StartDate)
                : Fail<Error, DateTime>($"Start date must not be in the past");

        private static Validation<Error, decimal> ValidateBudget(CreateDepartment createDepartment) =>
            AtLeast(0M)(createDepartment.Budget).Bind(AtMost(999999));

        private static Validation<Error, string> ValidateDepartmentName(CreateDepartment createDepartment) =>
            NotEmpty(createDepartment.Name).Bind(NotLongerThan(50));
    }
}
