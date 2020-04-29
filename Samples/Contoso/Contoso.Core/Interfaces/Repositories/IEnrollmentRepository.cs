using System.Threading.Tasks;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Core.Interfaces.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Option<Enrollment>> Get(int id);
        Task<int> Add(Enrollment enrollment);
        Task Update(Enrollment enrollment);
        Task Delete(int id);
    }
}
