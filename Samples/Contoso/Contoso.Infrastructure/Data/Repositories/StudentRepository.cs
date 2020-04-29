using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ContosoDbContext _contosoDbContext;

        public StudentRepository(ContosoDbContext dbContext)
        {
            _contosoDbContext = dbContext;
        }

        public async Task<int> Add(Student student)
        {
            await _contosoDbContext.Students.AddAsync(student);
            await _contosoDbContext.SaveChangesAsync();
            return student.StudentId;
        }

        public async Task<Option<Student>> Get(int Id) =>
            await _contosoDbContext.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .SingleOrDefaultAsync(s => s.StudentId == Id);

        public Task<List<Student>> GetAll() => 
            _contosoDbContext.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .ToListAsync();

        public async Task Update(Student student)
        {
            _contosoDbContext.Students.Update(student);
            await _contosoDbContext.SaveChangesAsync();
        }

        public async Task Delete(int studentId)
        {
            var student = await _contosoDbContext.Students.FindAsync(studentId);
            _contosoDbContext.Students.Remove(student);
            await _contosoDbContext.SaveChangesAsync();
        }
    }
}
