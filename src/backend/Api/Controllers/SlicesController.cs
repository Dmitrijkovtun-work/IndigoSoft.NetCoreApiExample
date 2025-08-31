using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SlicesController : ControllerBase
    {
        /// <summary>
        /// Post packet with slices
        /// </summary>
        /// <returns>
        /// 400
        /// </returns>
        
        // [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SlicesPacket()
        {
            await Task.Run(() => { Console.WriteLine(""); });
            // OkResult okResult = Ok();
            return Ok("dssds");
        }
    }
}
