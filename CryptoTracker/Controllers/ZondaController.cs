using Common.Connectors.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZondaController : ControllerBase
    {
        private readonly IZonda zonda;

        public ZondaController(IZonda zonda)
        {
            this.zonda = zonda;
        }
        [HttpGet]
        public async Task<IActionResult> TestZonda()
        {
            var a = await zonda.GetTransactionsAsync();

            return Ok(a);
        }
    }
}
