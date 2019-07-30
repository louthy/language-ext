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
            await _contosoDbContext.Students.FindAsync(Id);

        public Task<List<Student>> GetAll() => 
            _contosoDbContext.Students.ToListAsync();

        public async Task Update(Student student)
        {
            _contosoDbContext.Students.Update(student);
            await _contosoDbContext.SaveChangesAsync();
        }
    }
}
