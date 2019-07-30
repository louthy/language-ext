using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Queries
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudents, List<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        public GetAllStudentsHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<List<Student>> Handle(GetAllStudents request, CancellationToken cancellationToken) => 
            _studentRepository.GetAll();
    }
}
