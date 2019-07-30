using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Instructors.Queries
{
    public class GetInstructorByIdHandler : IRequestHandler<GetInstructorById, Option<InstructorViewModel>>
    {
        private readonly IInstructorRepository _instructorRepository;
        public GetInstructorByIdHandler(IInstructorRepository instructorRepository) => _instructorRepository = instructorRepository;

        public async Task<Option<InstructorViewModel>> Handle(GetInstructorById request, CancellationToken cancellationToken) => 
            (await _instructorRepository.Get(request.InstructorId)).Map(Project);

        private static InstructorViewModel Project(Instructor instructor) =>
            new InstructorViewModel(instructor.InstructorId, instructor.FirstName, 
                instructor.LastName, instructor.HireDate, 
                new Lst<CourseAssignmentViewModel>(ToViewModel(instructor.CourseAssignments)));

        private static IEnumerable<CourseAssignmentViewModel> ToViewModel(IEnumerable<CourseAssignment> courseAssignments) =>
            courseAssignments.Map(ca => Project(ca.Course));

        private static CourseAssignmentViewModel Project(Course course) =>
            new CourseAssignmentViewModel(course.CourseId, course.Title, course.Credits, course.DepartmentId);
    }
}
