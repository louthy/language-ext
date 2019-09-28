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
    public class UpdateDepartmentHandler : IRequestHandler<UpdateDepartment, Either<Error, Task>>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IInstructorRepository _instructorRepository;


        public UpdateDepartmentHandler(IDepartmentRepository departmentRepository,
            IInstructorRepository instructorRepository)
        {
            _departmentRepository = departmentRepository;
            _instructorRepository = instructorRepository;
        }

        public Task<Either<Error, Task>> Handle(UpdateDepartment request, CancellationToken cancellationToken) =>
            Validate(request)
                .MapT(d => ApplyUpdateRequest(d, request))
                .Map(v => v.ToEither<Task>());

        private Task ApplyUpdateRequest(Department d, UpdateDepartment update)
        {
            d.Name = update.Name;
            d.Budget = update.Budget;
            d.AdministratorId = update.AdministratorId;
            d.StartDate = update.StartDate;
            return _departmentRepository.Update(d);
        }

        private async Task<Validation<Error, Department>> Validate(UpdateDepartment update) =>
            (ValidateDepartmentName(update), ValidateBudget(update), MustStartInFuture(update),
            await InstructorIdMustExist(update), await DepartmentMustExist(update))
                .Apply((n, b, s, i, department) => department);

        private async Task<Validation<Error, int>> InstructorIdMustExist(UpdateDepartment updateDepartment) =>
            (await _instructorRepository.Get(updateDepartment.AdministratorId))
                .Map(i => i.InstructorId)
                .ToValidation<Error>($"Administrator Id {updateDepartment.AdministratorId} does not exist");

        private async Task<Validation<Error, Department>> DepartmentMustExist(UpdateDepartment updateDepartment) =>
            (await _departmentRepository.Get(updateDepartment.DepartmentId))
                .ToValidation<Error>($"Department Id: {updateDepartment.DepartmentId} does not exist.");

        private static Validation<Error, DateTime> MustStartInFuture(UpdateDepartment updateDepartment) =>
            updateDepartment.StartDate > DateTime.UtcNow
                ? Success<Error, DateTime>(updateDepartment.StartDate)
                : Fail<Error, DateTime>($"Start date must not be in the past");

        private static Validation<Error, decimal> ValidateBudget(UpdateDepartment updateDepartment) =>
            AtLeast(0M)(updateDepartment.Budget).Bind(AtMost(999999));

        private static Validation<Error, string> ValidateDepartmentName(UpdateDepartment updateDepartment) =>
            NotEmpty(updateDepartment.Name).Bind(NotLongerThan(50));
    }
}
