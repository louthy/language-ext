using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Application.Students.Mapper;

namespace Contoso.Application.Students.Queries
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentById, Option<StudentViewModel>>
    {
        private readonly IStudentRepository _studentRepository;
        public GetStudentByIdHandler(IStudentRepository studentRepository) => 
            _studentRepository = studentRepository;

        public Task<Option<StudentViewModel>> Handle(GetStudentById request, CancellationToken cancellationToken) =>
            _studentRepository.Get(request.StudentId)
                .MapT(ProjectToViewModel);
    }
}
