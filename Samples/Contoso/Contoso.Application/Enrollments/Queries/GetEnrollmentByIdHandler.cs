using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;

namespace Contoso.Application.Enrollments.Queries
{
    public class GetEnrollmentByIdHandler : IRequestHandler<GetEnrollmentById, Option<EnrollmentViewModel>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetEnrollmentByIdHandler(IEnrollmentRepository enrollmentRepository) => _enrollmentRepository = enrollmentRepository;

        public Task<Option<EnrollmentViewModel>> Handle(GetEnrollmentById request, CancellationToken cancellationToken) => 
            Fetch(request.EnrollmentId)
                .MapT(ProjectToViewModel);

        private Task<Option<Enrollment>> Fetch(int id) => _enrollmentRepository.Get(id);

        private static EnrollmentViewModel ProjectToViewModel(Enrollment enrollment) =>
            new EnrollmentViewModel(enrollment.EnrollmentId, enrollment.CourseId, 
                CourseName(enrollment), enrollment.StudentId, StudentName(enrollment), enrollment.Grade);

        private static string CourseName(Enrollment e) =>
            Optional(e.Course)
                .Map(c => c.Title)
                .IfNone("");

        private static string StudentName(Enrollment e) =>
            Optional(e.Student)
                .Map(s => s.FullName)
                .IfNone("");

    }
}
