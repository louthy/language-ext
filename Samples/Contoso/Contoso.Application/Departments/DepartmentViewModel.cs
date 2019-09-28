using System;
using LanguageExt;

namespace Contoso.Application.Departments
{
    public class DepartmentViewModel : Record<DepartmentViewModel>
    {
        public DepartmentViewModel(int departmentId, string name, decimal budget, DateTime startDate, int? administratorId)
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
        public int? AdministratorId { get; }
    }
}
