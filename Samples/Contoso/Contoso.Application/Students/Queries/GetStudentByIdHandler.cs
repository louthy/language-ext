using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Queries
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentById, Option<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        public GetStudentByIdHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<Option<Student>> Handle(GetStudentById request, CancellationToken cancellationToken) =>
            _studentRepository.Get(request.StudentId);
    }
}
