using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Departments.Commands
{
    public class DeleteDepartment : Record<DeleteDepartment>, IRequest<Either<Error, Task>>
    {
        public DeleteDepartment(int departmentId) => DepartmentId = departmentId;

        public int DepartmentId { get; }
    }
}
