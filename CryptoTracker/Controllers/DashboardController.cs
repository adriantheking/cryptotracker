using CryptoCommon.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> logger;
        private readonly IZondaService zondaService;

        public DashboardController(ILogger<DashboardController> logger, IZondaService zondaService)
        {
            this.logger = logger;
            this.zondaService = zondaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var zondaTransactions = await this.zondaService.GetTransactionsAsync();

            return Ok(zondaTransactions);
        }


    }
}
