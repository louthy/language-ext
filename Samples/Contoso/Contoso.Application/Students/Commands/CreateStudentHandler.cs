using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;
using static Contoso.Application.Validators.StringValidation;

namespace Contoso.Application.Students.Commands
{
    public class CreateStudentHandler : IRequestHandler<CreateStudent, Either<Error, int>>
    {
        private readonly IStudentRepository _studentRepository;

        public CreateStudentHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

        public Task<Either<Error, int>> Handle(CreateStudent request, CancellationToken cancellationToken) =>
            Validate(request)
                .Map(s => _studentRepository.Add(s))
                .MatchAsync<Either<Error, int>>(
                    SuccAsync: async studentId => Right(await studentId),
                    Fail: failures => Left(failures.Join()));

        private Validation<Error, Student> Validate(CreateStudent request) => 
            (ValidateFirstName(request), ValidateLastName(request), ValidateEnrollmentDate(request))
                .Apply((f, l, e) => new Student { FirstName = f, LastName = l, EnrollmentDate = e });

        private Validation<Error, string> ValidateFirstName(CreateStudent createStudent) =>
            NotEmpty(createStudent.FirstName)
                .Bind(firstName => MaxStringLength(50)(firstName));

        private Validation<Error, string> ValidateLastName(CreateStudent createStudent) =>
            NotEmpty(createStudent.LastName)
                .Bind(lastName => MaxStringLength(50)(lastName));

        private Validation<Error, DateTime> ValidateEnrollmentDate(CreateStudent createStudent) =>
            createStudent.EnrollmentDate > DateTime.Now.AddYears(5)
                ? Fail<Error, DateTime>($"The enrollment date is too far in the future")
                : Success<Error, DateTime>(createStudent.EnrollmentDate);
    }
}
