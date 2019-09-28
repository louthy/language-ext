using System.Threading.Tasks;
using Contoso.Application.Students.Commands;
using Contoso.Application.Students.Queries;
using Contoso.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{studentId}")]
        public Task<IActionResult> Get(int studentId) =>
            _mediator.Send(new GetStudentById(studentId)).ToActionResult();

        [HttpGet]
        public async Task<IActionResult> Index() =>
            Ok(await _mediator.Send(new GetAllStudents()));

        [HttpPost]
        public Task<IActionResult> Create([FromBody]CreateStudent createStudent) =>
            _mediator.Send(createStudent).ToActionResult();

        [HttpPut]
        public Task<IActionResult> Update([FromBody] UpdateStudent updateStudent) =>
            _mediator.Send(updateStudent).ToActionResult();

        [HttpDelete]
        public Task<IActionResult> Delete([FromBody] DeleteStudent deleteStudent) =>
            _mediator.Send(deleteStudent).ToActionResult();
    }
}
