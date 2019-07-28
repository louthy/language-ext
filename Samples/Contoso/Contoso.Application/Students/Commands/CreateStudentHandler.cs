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
            (ValidateFirstName(request) | ValidateLastName(request))
                .MatchAsync<Either<Error, int>>(
                    SuccAsync: async _ => Right(await Persist(request)),
                    Fail: failures => Left(failures.Join()));

        private Task<int> Persist(CreateStudent createStudent) => 
            _studentRepository.Add(new Student
                {
                    FirstName = createStudent.FirstName,
                    LastName = createStudent.LastName,
                    EnrollmentDate = createStudent.EnrollmentDate
                });

        private Validation<Error, string> ValidateFirstName(CreateStudent createStudent) =>
            NotEmpty(createStudent.FirstName)
                .Bind(firstName => MaxStringLength(50, firstName));

        private Validation<Error, string> ValidateLastName(CreateStudent createStudent) =>
            NotEmpty(createStudent.LastName)
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
