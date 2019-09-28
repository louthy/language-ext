using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Validators;

namespace Contoso.Application.Instructors.Commands
{
    public class CreateOfficeAssignmentHandler : IRequestHandler<CreateOfficeAssignment, Validation<Error, int>>
    {
        private readonly IOfficeAssignmentRepository _officeAssignmentRepository;
        private readonly IInstructorRepository _instructorRepository;

        public CreateOfficeAssignmentHandler(IOfficeAssignmentRepository officeAssignmentRepository,
            IInstructorRepository instructorRepository)
        {
            _officeAssignmentRepository = officeAssignmentRepository;
            _instructorRepository = instructorRepository;
        }

        public Task<Validation<Error, int>> Handle(CreateOfficeAssignment request, CancellationToken cancellationToken) => 
            Validate(request)
                .MapT(Persist)
                .Bind(v => v.Traverse(i => i));

        private Task<int> Persist(OfficeAssignment officeAssignment) => 
            _officeAssignmentRepository.Create(officeAssignment);

        private async Task<Validation<Error, OfficeAssignment>> Validate(CreateOfficeAssignment create) => 
            (NotEmpty(create.Location), await InstructorMustExist(create))
                .Apply((loc, id) => new OfficeAssignment { InstructorId = id, Location = loc });

        private Task<Validation<Error, int>> InstructorMustExist(CreateOfficeAssignment create) =>
            _instructorRepository.Get(create.InstructorId)
                .Map(o => o.ToValidation<Error>($"Instructor Id {create.InstructorId} does not exist.")
                    .Map(v => v.InstructorId));
    }
}
