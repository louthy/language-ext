using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class CourseAssignmentRepository : ICourseAssignmentRepository
    {
        private readonly ContosoDbContext _contosoDbContext;

        public CourseAssignmentRepository(ContosoDbContext contosoDbContext) => _contosoDbContext = contosoDbContext;

        public Task<List<CourseAssignment>> GetByCourseId(int courseId) => 
            _contosoDbContext.CourseAssignments.Where(c => c.CourseID == courseId)
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .ToListAsync();

        public async Task<int> Add(CourseAssignment courseAssignment)
        {
            await _contosoDbContext.CourseAssignments.AddAsync(courseAssignment);
            await _contosoDbContext.SaveChangesAsync();
            return courseAssignment.CourseAssignmentId;
        }
    }
}
