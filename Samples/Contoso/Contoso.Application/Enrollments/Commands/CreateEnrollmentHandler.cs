using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Enrollments.Commands
{
    public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollment, Validation<Error, int>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;

        public CreateEnrollmentHandler(IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            IStudentRepository studentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
        }

        public Task<Validation<Error, int>> Handle(CreateEnrollment request, CancellationToken cancellationToken) => 
            from validation in
                from v in Validate(request)
                select Persist(v)
            from result in validation.Traverse(i => i) // Await the inner task
                select result;

        private Task<int> Persist(Enrollment enrollment) => _enrollmentRepository.Add(enrollment);

        private async Task<Validation<Error, Enrollment>> Validate(CreateEnrollment create) => 
            (await CourseMustExist(create), await StudentMustExist(create))
                .Apply((c, s) => new Enrollment { CourseId = c, StudentId = s, Grade = create.Grade });

        private async Task<Validation<Error, int>> CourseMustExist(CreateEnrollment create) =>
            (await _courseRepository.Get(create.CourseId))
                .ToValidation<Error>($"Course Id: {create.CourseId} does not exist.")
                .Map(c => c.CourseId);

        private async Task<Validation<Error, int>> StudentMustExist(CreateEnrollment create) => 
            (await _studentRepository.Get(create.StudentId))
                .ToValidation<Error>($"Student {create.StudentId} does not exist.")
                .Map(s => s.StudentId);
    }
}
