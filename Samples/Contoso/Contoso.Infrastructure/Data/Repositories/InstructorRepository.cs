using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly ContosoDbContext contosoDbContext;
        public InstructorRepository(ContosoDbContext context) => contosoDbContext = context;

        public async Task<int> Add(Instructor instructor)
        {
            await contosoDbContext.Instructors.AddAsync(instructor);
            await contosoDbContext.SaveChangesAsync();
            return instructor.InstructorId;
        }

        public async Task<Option<Instructor>> Get(int id) => 
            await contosoDbContext.Instructors
                .Include(i => i.CourseAssignments)
                    .ThenInclude(c => c.Course)
                .SingleOrDefaultAsync(i => i.InstructorId == id);


    }
}
