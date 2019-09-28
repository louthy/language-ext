using System.Threading.Tasks;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Core.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Option<Course>> Get(int id);
        Task<int> Add(Course course);
        Task<Unit> Update(Course course);
        Task<Unit> Delete(int id);
    }
}
