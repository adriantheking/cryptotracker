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
        private readonly IDashboardService dashboardService;

        public DashboardController(ILogger<DashboardController> logger, IZondaService zondaService,
            IDashboardService dashboardService)
        {
            this.logger = logger;
            this.zondaService = zondaService;
            this.dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var zondaTransactions = await this.zondaService.GetTransactionsAsync();

            return Ok(zondaTransactions);
        }


        [HttpGet("GetWallet")]
        public async Task<object> GetWallet()
        {
            return await dashboardService.GetWalletAsync();
        }
        [HttpGet("SyncWallet")]
        public async Task<object> SyncWallet()
        {
            return await dashboardService.SyncWalletAsync("1111");
        }
    }
}
