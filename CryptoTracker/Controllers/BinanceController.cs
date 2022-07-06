using Binance.Spot.Models;
using CryptoCommon.Connectors;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BinanceController : ControllerBase
    {
        private readonly IBinance binanceConnector;
        private readonly IBinanceService binanceService;

        public BinanceController(IBinance binanceConnector,
            IBinanceService binanceService)
        {
            this.binanceConnector = binanceConnector;
            this.binanceService = binanceService;
        }

        [HttpGet("c2cHistory")]
        public async Task<object> Index()
        {
            return (await this.binanceService.GetC2CTradeHistoryAsync(Side.BUY));
            //return (await this.binanceConnector.GetC2CHistoryAsync(Side.BUY));
        }
    }
}
