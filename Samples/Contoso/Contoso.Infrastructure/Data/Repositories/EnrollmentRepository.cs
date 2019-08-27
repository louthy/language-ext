using System;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ContosoDbContext _contosoDbContext;

        public EnrollmentRepository(ContosoDbContext contosoDbContext) => _contosoDbContext = contosoDbContext;

        public Task<int> Add(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Option<Enrollment>> Get(int id) => 
            await _contosoDbContext.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .SingleOrDefaultAsync(e => e.EnrollmentId == id);

        public Task Update(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }
    }
}
