using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Application.Students.Mapper;

namespace Contoso.Application.Students.Queries
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudents, List<StudentViewModel>>
    {
        private readonly IStudentRepository _studentRepository;
        public GetAllStudentsHandler(IStudentRepository studentRepository) => 
            _studentRepository = studentRepository;

        public async Task<List<StudentViewModel>> Handle(GetAllStudents request, CancellationToken cancellationToken) => 
            (await _studentRepository.GetAll()).Map(ProjectToViewModel).ToList();
    }
}
