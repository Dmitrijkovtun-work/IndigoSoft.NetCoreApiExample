using Example.Application.UseCases.Login;
using Example.Infrastructure.AccountService;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Example.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest credentials)
        {
            // Черновая версия
            /*
                var creds = await mediator.Send(credentials);
                return Ok(creds);
            */

            throw new NotImplementedException();
        }
    }
}
