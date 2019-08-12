using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IInstructorRepository _instructorRepository;

        public CreateDepartmentHandler(IDepartmentRepository departmentRepository,
            IInstructorRepository instructorRepository)
        {
            _departmentRepository = departmentRepository;
            _instructorRepository = instructorRepository;
        }

        public Task<Either<Error, int>> Handle(CreateDepartment request, CancellationToken cancellationToken) =>
            Validate(request)
                .Bind(v => v.Map(PersistDepartment).ToEitherAsync());

        private Task<int> PersistDepartment(Department d) => _departmentRepository.Add(d);

        private async Task<Validation<Error, Department>> Validate(CreateDepartment create) => 
            (ValidateDepartmentName(create), ValidateBudget(create), 
                MustStartInFuture(create), await InstructorIdMustExist(create))
                .Apply((n, b, s, i) => new Department { Name = n, Budget = b, StartDate = s, AdministratorId = i });

        private Task<Validation<Error, int>> InstructorIdMustExist(CreateDepartment createDepartment) =>
            _instructorRepository.Get(createDepartment.AdministratorId).Match(
                Some: s => s.InstructorId,
                None: () => Fail<Error, int>($"Administrator Id {createDepartment.AdministratorId} does not exist"));

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
