using Example.Infrastructure.AccountService;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Example.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto credentials)
        {
            var _ = await mediator.Send(credentials);
            return Ok(credentials);
        }
    }
}
