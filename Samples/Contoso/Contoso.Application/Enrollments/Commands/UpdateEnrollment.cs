using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Enrollments.Commands
{
    public class UpdateEnrollment : Record<UpdateEnrollment>, IRequest<Validation<Error, Task>>
    {
        public UpdateEnrollment(int enrollmentId, Grade? grade)
        {
            EnrollmentId = enrollmentId;
            Grade = grade;
        }

        public int EnrollmentId { get; }
        public Grade? Grade { get; }
    }
}
