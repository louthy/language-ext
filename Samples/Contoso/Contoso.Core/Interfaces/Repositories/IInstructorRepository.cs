using System.Threading.Tasks;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Core.Interfaces.Repositories
{
    public interface IInstructorRepository
    {
        Task<Option<Instructor>> Get(int id);
        Task<int> Add(Instructor instructor);
        Task Update(Instructor instructor);
        Task Delete(int id);
    }
}
