using Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZondaController : ControllerBase
    {
        private readonly IZondaService zondaService;

        public ZondaController(IZondaService zondaService)
        {
            this.zondaService = zondaService;
        }

        [HttpGet(nameof(GetZondaOperations))]
        public async Task<IActionResult> GetZondaOperations()
        {
            var zondaOperations = await this.zondaService.GetOperationsAsync();

            return Ok(zondaOperations);
        }

        [HttpGet(nameof(GetInvestedAmount))]
        public async Task<IActionResult> GetInvestedAmount()
        {
            var investedAmount = await this.zondaService.GetInvestedAmountAsync();
            return Ok(investedAmount);
        }

        [HttpGet(nameof(GetCryptoBalance))]
        public async Task<IActionResult> GetCryptoBalance()
        {
            return Ok(await this.zondaService.GetCryptoBalancesAsync());
        }
    }
}
