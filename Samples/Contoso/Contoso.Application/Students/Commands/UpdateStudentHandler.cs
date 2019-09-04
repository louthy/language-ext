using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;

namespace Contoso.Application.Students.Commands
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudent, Either<Error, Task>>
    {
        private readonly IStudentRepository _studentRepository;
        public UpdateStudentHandler(IStudentRepository studentRepository) => 
            _studentRepository = studentRepository;

        public async Task<Either<Error, Task>> Handle(UpdateStudent request, CancellationToken cancellationToken) => 
            (await Validate(request))
                .Map(s => ApplyUpdateRequest(s, request))
                .ToEither<Task>();

        private async Task ApplyUpdateRequest(Student s, UpdateStudent update)
        {
            s.FirstName = update.FirstName;
            s.LastName = update.LastName;
            await _studentRepository.Update(s);
        }

        private async Task<Validation<Error, Student>> Validate(UpdateStudent request) =>
            (ValidateFirstName(request), ValidateLastName(request), await StudentMustExist(request))
                .Apply((first, last, studentToUpdate) => studentToUpdate);

        private async Task<Validation<Error, Student>> StudentMustExist(UpdateStudent updateStudent) =>
            (await _studentRepository.Get(updateStudent.StudentId))
                .ToValidation<Error>("Student does not exist.");

        private Validation<Error, string> ValidateFirstName(UpdateStudent updateStudent) =>
            NotEmpty(updateStudent.FirstName)
                .Bind(firstName => MaxStringLength(50, firstName));

        private Validation<Error, string> ValidateLastName(UpdateStudent updateStudent) =>
            NotEmpty(updateStudent.LastName)
                .Bind(lastName => MaxStringLength(50, lastName));

        private Validation<Error, string> MaxStringLength(int maxLength, string str) =>
            str.Length > maxLength
                ? Fail<Error, string>($"{str} must not be longer than {maxLength}")
                : Success<Error, string>(str);

        private Validation<Error, string> NotEmpty(string str) =>
            string.IsNullOrEmpty(str)
                ? Fail<Error, string>("Must not be empty")
                : Success<Error, string>(str);
    }
}
