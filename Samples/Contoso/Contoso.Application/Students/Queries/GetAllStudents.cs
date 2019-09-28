using System.Collections.Generic;
using Contoso.Core.Domain;
using MediatR;

namespace Contoso.Application.Students.Queries
{
    public class GetAllStudents : IRequest<List<StudentViewModel>>
    { }
}
