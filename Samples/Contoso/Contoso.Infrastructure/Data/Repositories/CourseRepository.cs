using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ContosoDbContext contosoDbContext;

        public CourseRepository(ContosoDbContext contosoDbContext) => 
            this.contosoDbContext = contosoDbContext;

        public async Task<int> Add(Course course)
        {
            await contosoDbContext.Courses.AddAsync(course);
            await contosoDbContext.SaveChangesAsync();
            return course.CourseId;
        }

        public async Task Delete(int id)
        {
            var course = await contosoDbContext.Courses.FindAsync(id);
            contosoDbContext.Courses.Remove(course);
            await contosoDbContext.SaveChangesAsync();
        }

        public async Task<Option<Course>> Get(int id) => 
            await contosoDbContext.Courses.SingleOrDefaultAsync(c => c.CourseId == id);

        public Task Update(Course course)
        {
            contosoDbContext.Courses.Update(course);
            return contosoDbContext.SaveChangesAsync();
        }
    }
}
