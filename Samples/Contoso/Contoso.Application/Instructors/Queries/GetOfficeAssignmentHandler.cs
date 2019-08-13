using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Instructors.Queries
{
    public class GetOfficeAssignmentHandler : IRequestHandler<GetOfficeAssignment, Option<OfficeAssignment>>
    {
        private readonly IOfficeAssignmentRepository _officeAssignmentRepository;

        public GetOfficeAssignmentHandler(IOfficeAssignmentRepository officeAssignmentRepository) => _officeAssignmentRepository = officeAssignmentRepository;

        public Task<Option<OfficeAssignment>> Handle(GetOfficeAssignment request, CancellationToken cancellationToken) => 
            Fetch(request.InstructorId).Map(Project);

        private Option<OfficeAssignment> Project(Option<OfficeAssignment> o) =>
            o.Map(oa => new OfficeAssignment
                {
                    OfficeAssignmentId = oa.OfficeAssignmentId,
                    InstructorId = oa.InstructorId,
                    Location = oa.Location
                });

        private Task<Option<OfficeAssignment>> Fetch(int id) => 
            _officeAssignmentRepository.GetByInstructorId(id);
    }
}
