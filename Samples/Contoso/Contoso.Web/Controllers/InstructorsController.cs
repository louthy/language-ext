using System.Threading.Tasks;
using Contoso.Application.Instructors.Commands;
using Contoso.Application.Instructors.Queries;
using Contoso.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InstructorsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{instructorId}")]
        public Task<IActionResult> Get(int instructorId) =>
            _mediator.Send(new GetInstructorById(instructorId)).ToActionResult();

        [HttpPost]
        public Task<IActionResult> Create([FromBody] CreateInstructor createInstructor) =>
            _mediator.Send(createInstructor).ToActionResult();

        [HttpPut]
        public Task<IActionResult> Update([FromBody] UpdateInstructor updateInstructor) =>
            _mediator.Send(updateInstructor).ToActionResult();

        [HttpDelete]
        public Task<IActionResult> Delete([FromBody] DeleteInstructor deleteInstructor) =>
            _mediator.Send(deleteInstructor).ToActionResult();

        [HttpGet("officeassignment/{instructorId}")]
        public Task<IActionResult> GetOfficeAssignment(int instructorId) =>
            _mediator.Send(new GetOfficeAssignment(instructorId)).ToActionResult();

        [HttpPost("officeassignment")]
        public async Task<IActionResult> CreateOfficeAssignment([FromBody] CreateOfficeAssignment create) =>
            await _mediator.Send(create).ToActionResult();
    }
}
