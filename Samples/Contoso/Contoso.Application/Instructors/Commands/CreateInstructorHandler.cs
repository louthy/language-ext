using System;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Application.Instructors.Commands
{
    public class CreateInstructorHandler : IRequestHandler<CreateInstructor, Either<Error, int>>
    {
        private readonly IInstructorRepository _instructorRepository;
        public CreateInstructorHandler(IInstructorRepository instructorRepository) => _instructorRepository = instructorRepository;

        public Task<Either<Error, int>> Handle(CreateInstructor request, CancellationToken cancellationToken) => 
            Validate(request)
                .Map(i => _instructorRepository.Add(i))
                .MatchAsync<Either<Error, int>>(
                    SuccAsync: async id => Right(await id),
                    Fail: err => Left(err.Join()));

        private Validation<Error, Instructor> Validate(CreateInstructor command) =>
            (ValidateFirstName(command), ValidateLastName(command), ValidateHireDate(command))
                .Apply((f, s, h) => new Instructor { FirstName = f, LastName = s, HireDate = h });

        private Validation<Error, string> ValidateFirstName(CreateInstructor createStudent) =>
            NotEmpty(createStudent.FirstName)
                .Bind(firstName => NotLongerThan(50)(firstName));

        private Validation<Error, string> ValidateLastName(CreateInstructor createStudent) =>
            NotEmpty(createStudent.LastName)
                .Bind(lastName => NotLongerThan(50)(lastName));

        private Validation<Error, DateTime> ValidateHireDate(CreateInstructor createStudent) =>
            createStudent.HireDate > DateTime.Now.AddYears(5)
                ? Fail<Error, DateTime>($"The enrollment date is too far in the future")
                : Success<Error, DateTime>(createStudent.HireDate);
    }
}
