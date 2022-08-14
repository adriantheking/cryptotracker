using Binance.Spot.Models;
using CryptoCommon.Connectors;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using CryptoCommon.Utilities;
using LazyCache;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BinanceController : ControllerBase
    {
        private readonly IAppCache cache;
        private readonly IBinanceService binanceService;
        private readonly IBinanceSeed seed;

        public BinanceController(ILogger<BinanceController> logger,
            IAppCache cache,
            IBinanceService binanceService,
            IBinanceSeed seed)
        {
            this.cache = cache;
            this.binanceService = binanceService;
            this.seed = seed;
        }

        [HttpGet("c2cHistory")]
        public async Task<object> Index()
        {
            return Ok();
        }

        [HttpGet("GetInvestedAmount")]
        public async Task<object> GetInvestedAmount()
        {
            return Ok();
        }

        [HttpGet("GetOrders")]
        public async Task<object> GetOrders()
        {
            return await binanceService.GetSpotOrdersHistoryAsync(new List<string> { "BTCUSDT" });
        }

        [HttpGet("GetTrades")]
        public async Task<object> GetTrades()
        {
            return await binanceService.GetSpotTradesHistoryAsync(new List<string> { "BTCUSDT", "ETHUSDT" });
        }


        [HttpGet("Seed")]
        public async Task Seed()
        {
            await seed.Init();
        }
    }
}
