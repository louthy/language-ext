using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class OfficeAssignmentRepository : IOfficeAssignmentRepository
    {
        private readonly ContosoDbContext contosoDbContext;

        public OfficeAssignmentRepository(ContosoDbContext dbContext) => contosoDbContext = dbContext;

        public Task<int> Create(OfficeAssignment officeAssignment)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Option<OfficeAssignment>> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Option<OfficeAssignment>> GetByInstructorId(int instructorId) => 
            await contosoDbContext.OfficeAssignments
                .SingleOrDefaultAsync(o => o.InstructorId == instructorId);

        public Task Update(OfficeAssignment officeAssignment)
        {
            throw new NotImplementedException();
        }
    }
}
