using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using static LanguageExt.Prelude;
using MediatR;

namespace Contoso.Application.Students.Commands
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudent, Either<Error, Task>>
    {
        private readonly IStudentRepository _studentRepository;
        public DeleteStudentHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

        public async Task<Either<Error, Task>> Handle(DeleteStudent request, CancellationToken cancellationToken) => 
            (await StudentMustExist(request))
                .Map(d => _studentRepository.Delete(d.StudentId))
                .Match<Either<Error, Task>>(
                    Succ: t => Right(t),
                    Fail: errors => Left(errors.Join()));

        private async Task<Validation<Error, DeleteStudent>> StudentMustExist(DeleteStudent deleteStudent) => 
            (await _studentRepository.Get(deleteStudent.StudentId)).Match(
                Some: _ => Success<Error, DeleteStudent>(deleteStudent),
                None: () => Fail<Error, DeleteStudent>($"Student {deleteStudent.StudentId} does not exist."));
    }
}
