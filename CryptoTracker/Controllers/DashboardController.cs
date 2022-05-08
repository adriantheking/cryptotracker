using Common.Connectors.Interfaces;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> logger;
        private readonly IZondaService transactionService;

        public DashboardController(ILogger<DashboardController> logger, IZondaService zondaService)
        {
            this.logger = logger;
            this.transactionService = zondaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var zondaTransactions = await this.transactionService.GetTransactionsAsync();

            return Ok(zondaTransactions);
        }

        [HttpGet(nameof(GetOperations))]
        public async Task<IActionResult> GetOperations()
        {
            var zondaOperations = await this.transactionService.GetOperationsAsync();

            return Ok(zondaOperations);
        }
    } 
}
