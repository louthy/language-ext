using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Departments.Queries
{
    public class GetDepartmentByIdHandler : IRequestHandler<GetDepartmentById, Option<DepartmentViewModel>>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetDepartmentByIdHandler(IDepartmentRepository departmentRepository) => 
            _departmentRepository = departmentRepository;

        public Task<Option<DepartmentViewModel>> Handle(GetDepartmentById request, CancellationToken cancellationToken) => 
            _departmentRepository.Get(request.DepartmentId).MapT(d => Project(d));

        private static DepartmentViewModel Project(Department d) =>
            new DepartmentViewModel(d.DepartmentId, d.Name, d.Budget, d.StartDate, d.AdministratorId);
    }
}
