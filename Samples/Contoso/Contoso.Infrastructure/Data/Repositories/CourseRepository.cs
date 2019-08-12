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
    public class CourseRepository : ICourseRepository
    {
        private readonly ContosoDbContext contosoDbContext;

        public CourseRepository(ContosoDbContext contosoDbContext) => 
            this.contosoDbContext = contosoDbContext;

        public Task<int> Create(Course course)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Option<Course>> Get(int id) => 
            await contosoDbContext.Courses.SingleOrDefaultAsync(c => c.CourseId == id);

        public Task Update(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
