using System.Linq;
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
    public class CreateStudentHandler : IRequestHandler<CreateStudent, Either<Error, int>>
    {
        private readonly IStudentRepository _studentRepository;

        public CreateStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<Either<Error, int>> Handle(CreateStudent request, CancellationToken cancellationToken) => 
            ValidateFirstName(request)
                .Bind(ValidateLastName)
                .MatchAsync<Either<Error, int>>(
                    SuccAsync: async command => Right(await Persist(command)),
                    Fail: failures => Left(failures.Join()));

        private Task<int> Persist(CreateStudent createStudent) => 
            _studentRepository.Add(new Student
                {
                    FirstName = createStudent.FirstName,
                    LastName = createStudent.LastName,
                    EnrollmentDate = createStudent.EnrollmentDate
                });

        private Validation<Error, CreateStudent> ValidateFirstName(CreateStudent createStudent) =>
            (NotEmpty(createStudent.FirstName) | MaxStringLength(50, createStudent.FirstName))
                .Apply(c => createStudent);

        private Validation<Error, CreateStudent> ValidateLastName(CreateStudent createStudent) =>
            (NotEmpty(createStudent.LastName) | MaxStringLength(50, createStudent.LastName))
                .Apply(c => createStudent);

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
