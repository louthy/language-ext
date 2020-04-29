using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using static LanguageExt.Prelude;
using MediatR;

namespace Contoso.Application.Courses.Queries
{
    public class GetCourseAssignmentsHandler : IRequestHandler<GetCourseAssignments, List<CourseAssignmentViewModel>>
    {
        private readonly ICourseAssignmentRepository _courseAssignmentRepository;

        public GetCourseAssignmentsHandler(ICourseAssignmentRepository courseAssignmentRepository) => _courseAssignmentRepository = courseAssignmentRepository;

        public Task<List<CourseAssignmentViewModel>> Handle(GetCourseAssignments request, CancellationToken cancellationToken) => 
            Fetch(request.CourseId)
                .Map(Project);

        private Task<List<CourseAssignment>> Fetch(int courseId) => 
            _courseAssignmentRepository.GetByCourseId(courseId);

        private List<CourseAssignmentViewModel> Project(List<CourseAssignment> courseAssignments) => 
            courseAssignments.Map(ca => 
                new CourseAssignmentViewModel(
                    ca.CourseAssignmentId, 
                    ca.InstructorID, 
                    ca.CourseID, 
                    InstructorName(ca.Instructor), 
                    ca.Course.Title))
            .ToList();

        private string InstructorName(Instructor instructor) =>
            Optional(instructor)
                .Map(i => $"{i.FirstName} {i.LastName}")
                .IfNone("");
    }
}
