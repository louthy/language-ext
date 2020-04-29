using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;

namespace Contoso.Application.Students.Commands
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudent, Either<Error, Task>>
    {
        private readonly IStudentRepository _studentRepository;
        public DeleteStudentHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

        public async Task<Either<Error, Task>> Handle(DeleteStudent request, CancellationToken cancellationToken) =>
            (await StudentMustExist(request))
                .Map(DoDeletion)
                .ToEither<Task>();

        private Task DoDeletion(int studentId) =>
            _studentRepository.Delete(studentId);

        private async Task<Validation<Error, int>> StudentMustExist(DeleteStudent deleteStudent) => 
            (await _studentRepository.Get(deleteStudent.StudentId))
                .ToValidation<Error>($"Student {deleteStudent.StudentId} does not exist.")
                .Map(s => s.StudentId);
    }
}
