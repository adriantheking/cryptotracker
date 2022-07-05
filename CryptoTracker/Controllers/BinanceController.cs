using CryptoCommon.Connectors;
using CryptoCommon.Connectors.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BinanceController : ControllerBase
    {
        private readonly IBinance binanceConnector;

        public BinanceController(IBinance binanceConnector)
        {
            this.binanceConnector = binanceConnector;
        }

        [HttpGet("c2cHistory")]
        public async Task<object> Index()
        {
            return (await this.binanceConnector.GetC2CHistoryAsync());
        }
    }
}
