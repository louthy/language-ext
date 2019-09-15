using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;

namespace Contoso.Application.Enrollments.Commands
{
    public class UpdateEnrollmentHandler : IRequestHandler<UpdateEnrollment, Validation<Error, Task>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public UpdateEnrollmentHandler(IEnrollmentRepository enrollmentRepository) => _enrollmentRepository = enrollmentRepository;

        public async Task<Validation<Error, Task>> Handle(UpdateEnrollment request, CancellationToken cancellationToken) =>
            (await Validate(request))
                .Map(e => Apply(e, request))
                .Map(Persist);

        private Task Persist(Enrollment e) => _enrollmentRepository.Update(e);

        private static Enrollment Apply(Enrollment e, UpdateEnrollment update) =>
            new Enrollment
            {
                EnrollmentId = e.EnrollmentId,
                Grade = update.Grade,
                CourseId = e.CourseId,
                StudentId = e.StudentId
            };

        private async Task<Validation<Error, Enrollment>> Validate(UpdateEnrollment update) =>
            (await EnrollmentMustExist(update), ValidateGrade(update))
                .Apply((e, g) => e);

        private static Validation<Error, Grade> ValidateGrade(UpdateEnrollment update) =>
            Optional(update.Grade)
                .ToValidation<Error>("Invalid Grade.");

        private async Task<Validation<Error, Enrollment>> EnrollmentMustExist(UpdateEnrollment update) =>
            (await _enrollmentRepository.Get(update.EnrollmentId))
                .ToValidation<Error>($"Enrollment Id: {update.EnrollmentId} does not exist.");
    }
}
