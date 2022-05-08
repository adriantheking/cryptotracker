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
        private readonly ITransactionService transactionService;

        public DashboardController(ILogger<DashboardController> logger, ITransactionService transactionService)
        {
            this.logger = logger;
            this.transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var zondaTransactions = await this.transactionService.GetZondaTransactions();

            return Ok(zondaTransactions);
        }
    }
}
