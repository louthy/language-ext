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

        public async Task<int> Add(Enrollment enrollment)
        {
            _contosoDbContext.Enrollments.Add(enrollment);
            await _contosoDbContext.SaveChangesAsync();
            return enrollment.EnrollmentId;
        }

        public async Task Delete(int id)
        {
            var enrollment = await _contosoDbContext.Enrollments.FindAsync(id);
            _contosoDbContext.Enrollments.Remove(enrollment);
            await _contosoDbContext.SaveChangesAsync();
        }

        public async Task<Option<Enrollment>> Get(int id) => 
            await _contosoDbContext.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .SingleOrDefaultAsync(e => e.EnrollmentId == id);

        public async Task Update(Enrollment enrollment)
        {
            _contosoDbContext.Enrollments.Update(enrollment);
            await _contosoDbContext.SaveChangesAsync();
        }
    }
}
