using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Enrollments.Commands
{
    public class DeleteEnrollment : Record<DeleteEnrollment>, IRequest<Validation<Error, Task>>
    {
        public DeleteEnrollment(int enrollmentId) => EnrollmentId = enrollmentId;

        public int EnrollmentId { get; }
    }
}
