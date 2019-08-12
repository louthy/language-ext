using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Core.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Option<Course>> Get(int id);
        Task<int> Create(Course course);
        Task Update(Course course);
        Task Delete(int id);
    }
}
