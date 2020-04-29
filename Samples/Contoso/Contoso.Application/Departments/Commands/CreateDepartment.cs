using System;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Departments.Commands
{
    public class CreateDepartment : Record<CreateDepartment>, IRequest<Either<Error, int>>
    {
        public CreateDepartment(string name, decimal budget, DateTime startDate, int administratorId)
        {
            Name = name;
            Budget = budget;
            StartDate = startDate;
            AdministratorId = administratorId;
        }

        public string Name { get; }
        public decimal Budget { get; }
        public DateTime StartDate { get; }
        public int AdministratorId { get; }
    }
}
