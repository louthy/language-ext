using LanguageExt;
using MediatR;

namespace Contoso.Application.Enrollments.Queries
{
    public class GetEnrollmentById : Record<GetEnrollmentById>, IRequest<Option<EnrollmentViewModel>>
    {
        public GetEnrollmentById(int enrollmentId) => EnrollmentId = enrollmentId;

        public int EnrollmentId { get; }
    }
}
