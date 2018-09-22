using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Registration.Contracts;
using Registration.Data;
using Registration.Data.Models;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Registration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IBus _bus;

        public RegistrationController(IBus bus)
        {
            _bus = bus;
        }

        // GET: api/Registration/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid submissionId)
        {
            RegistrationStateReader reader = new RegistrationStateReader(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=RegistrationDemo;Integrated Security=True");
            try
            {
                RegistrationModel registration = await reader.Get(submissionId);
                return Ok(registration);
            }
            catch (Exception ex)
            {
                WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        // POST: api/Registration
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistrationModel registration)
        {
            if (registration == null)
            {
                return BadRequest();
            }

            await _bus.Send<ISubmitRegistration>(registration);

            RegistrationResponseModel response = new RegistrationResponseModel() { SubmissionId = registration.SubmissionId };

            return Ok();
        }
    }

    public class RegistrationResponseModel
    {
        public Guid SubmissionId { get; set; }
    }
}