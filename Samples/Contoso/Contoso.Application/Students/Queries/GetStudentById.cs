using LanguageExt;
using MediatR;

namespace Contoso.Application.Students.Queries
{
    public class GetStudentById : Record<GetStudentById>, IRequest<Option<StudentViewModel>>
    {
        public GetStudentById(int id) => StudentId = id;

        public int StudentId { get; }
    }
}
