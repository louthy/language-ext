using LanguageExt;
using MediatR;

namespace Contoso.Application.Departments.Queries
{
    public class GetDepartmentById : Record<GetDepartmentById>, IRequest<Option<DepartmentViewModel>>
    {
        public GetDepartmentById(int departmentId) => DepartmentId = departmentId;

        public int DepartmentId { get; }
    }
}
