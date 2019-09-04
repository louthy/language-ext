using Contoso.Core;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Instructors.Commands
{
    public class CreateOfficeAssignment: Record<CreateOfficeAssignment>, IRequest<Validation<Error, int>>
    {
        public CreateOfficeAssignment(int instructorId, string location)
        {
            InstructorId = instructorId;
            Location = location;
        }

        public int InstructorId { get; }
        public string Location { get; }
    }
}
