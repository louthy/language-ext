using System.Threading.Tasks;
using Contoso.Application.Enrollments.Commands;
using Contoso.Application.Enrollments.Queries;
using Contoso.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollmentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{enrollmentId}")]
        public Task<IActionResult> Get(int enrollmentId) => 
            _mediator.Send(new GetEnrollmentById(enrollmentId)).ToActionResult();
        
        [HttpPost]
        public Task<IActionResult> Create(CreateEnrollment create) =>
            _mediator.Send(create).ToActionResult();

        [HttpPut]
        public Task<IActionResult> Update(UpdateEnrollment update) =>
            _mediator.Send(update).ToActionResult();

        [HttpDelete]
        public Task<IActionResult> Delete(DeleteEnrollment delete) =>
            _mediator.Send(delete).ToActionResult();
    }
}
