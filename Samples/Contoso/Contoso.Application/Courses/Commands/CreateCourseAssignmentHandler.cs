using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Courses.Commands
{
    public class CreateCourseAssignmentHandler : IRequestHandler<CreateCourseAssignment, Validation<Error, int>>
    {
        private readonly IInstructorRepository _instructorRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseAssignmentRepository _courseAssignmentRepository;

        public CreateCourseAssignmentHandler(IInstructorRepository instructorRepository,
            ICourseRepository courseRepository,
            ICourseAssignmentRepository courseAssignmentRepository)
        {
            _instructorRepository = instructorRepository;
            _courseRepository = courseRepository;
            _courseAssignmentRepository = courseAssignmentRepository;
        }

        public Task<Validation<Error, int>> Handle(CreateCourseAssignment request, CancellationToken cancellationToken) =>
            Validate(request)
                .Bind(v => v.Map(Persist).Traverse(i => i));

        private Task<int> Persist(CourseAssignment courseAssignment) => _courseAssignmentRepository.Add(courseAssignment);

        private async Task<Validation<Error, CourseAssignment>> Validate(CreateCourseAssignment create) =>
            (await CourseMustExist(create), await InstructorMustExist(create))
                .Apply((c, i) => new CourseAssignment { CourseID = c.CourseId, InstructorID = i.InstructorId });

        private Task<Validation<Error, Course>> CourseMustExist(CreateCourseAssignment create) => 
            _courseRepository.Get(create.CourseId)
                .Map(o => o.ToValidation<Error>($"Course Id {create.CourseId} does not exist."));

        private Task<Validation<Error, Instructor>> InstructorMustExist(CreateCourseAssignment create) =>
            _instructorRepository.Get(create.InstructorId)
                .Map(o => o.ToValidation<Error>($"Instructor Id {create.InstructorId} does not exist."));
    }
}
