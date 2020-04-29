using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Infrastructure.Data.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ContosoDbContext contosoDbContext;
        public DepartmentRepository(ContosoDbContext dbContext) => contosoDbContext = dbContext;

        public async Task<int> Add(Department department)
        {
            await contosoDbContext.Departments.AddAsync(department);
            await contosoDbContext.SaveChangesAsync();
            return department.DepartmentId;
        }

        public async Task Delete(int departmentId)
        {
            var department = await contosoDbContext.Departments.FindAsync(departmentId);
            contosoDbContext.Departments.Remove(department);
            await contosoDbContext.SaveChangesAsync();
        }

        public async Task<Option<Department>> Get(int id) => 
            await contosoDbContext.Departments
                .SingleOrDefaultAsync(d => d.DepartmentId == id);

        public async Task Update(Department department)
        {
            contosoDbContext.Departments.Update(department);
            await contosoDbContext.SaveChangesAsync();
        }
    }
}
