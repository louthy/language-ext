using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Departments.Commands
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartment, Either<Error, Task>>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DeleteDepartmentHandler(IDepartmentRepository departmentRepository) => 
            _departmentRepository = departmentRepository;

        public Task<Either<Error, Task>> Handle(DeleteDepartment request, CancellationToken cancellationToken) =>
            DepartmentMustExist(request)
                .MapT(DoDeletion)
                .Map(v => v.ToEither<Task>());

        private Task DoDeletion(int departmentId) =>
            _departmentRepository.Delete(departmentId);

        private async Task<Validation<Error, int>> DepartmentMustExist(DeleteDepartment deleteDepartment) =>
            (await _departmentRepository.Get(deleteDepartment.DepartmentId))
                .Map(d => d.DepartmentId)
                .ToValidation<Error>($"Department Id {deleteDepartment.DepartmentId} does not exist.");
    }
}
