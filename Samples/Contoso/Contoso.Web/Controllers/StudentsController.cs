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

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{studentId}")]
        public Task<IActionResult> Get(int studentId) => 
            _mediator.Send(new GetStudentById(studentId))
                .ToActionResult();

        [HttpPost]
        public Task<IActionResult> Create([FromBody]CreateStudent createStudent) => 
            _mediator.Send(createStudent).ToActionResult();
    }
}
