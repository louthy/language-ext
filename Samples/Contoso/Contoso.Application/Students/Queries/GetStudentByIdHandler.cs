using System.Threading;
using System.Threading.Tasks;
using Contoso.Application.Courses;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Queries
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentById, Option<StudentViewModel>>
    {
        private readonly IStudentRepository _studentRepository;
        public GetStudentByIdHandler(IStudentRepository studentRepository) => 
            _studentRepository = studentRepository;

        public async Task<Option<StudentViewModel>> Handle(GetStudentById request, CancellationToken cancellationToken) =>
            (await _studentRepository.Get(request.StudentId)).Map(Project);

        private static StudentViewModel Project(Student student) =>
            new StudentViewModel(student.StudentId, student.FirstName, student.LastName,
                student.EnrollmentDate, new Lst<CourseViewModel>(student.Enrollments.Map(c => Project(c.Course))));

        private static CourseViewModel Project(Course course) =>
            new CourseViewModel(course.CourseId, course.Title, course.Credits, course.DepartmentId);
    }
}
