using System;
using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Departments.Commands
{
    public class UpdateDepartment : Record<UpdateDepartment>, IRequest<Either<Error, Task>>
    {
        public UpdateDepartment(int departmentId, string name, decimal budget, DateTime startDate, int administratorId)
        {
            DepartmentId = departmentId;
            Name = name;
            Budget = budget;
            StartDate = startDate;
            AdministratorId = administratorId;
        }

        public int DepartmentId { get; }
        public string Name { get; }
        public decimal Budget { get; }
        public DateTime StartDate { get; }
        public int AdministratorId { get; }
    }
}
