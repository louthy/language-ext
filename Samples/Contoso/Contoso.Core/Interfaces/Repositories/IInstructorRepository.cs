using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Core.Interfaces.Repositories
{
    public interface IInstructorRepository
    {
        Task<Option<Instructor>> Get(int id);
    }
}
