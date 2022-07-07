using Binance.Spot.Models;
using CryptoCommon.Connectors;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
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

        public BinanceController(ILogger<BinanceController> logger,
            IAppCache cache,
            IBinanceService binanceService)
        {
            this.cache = cache;
            this.binanceService = binanceService;
        }

        [HttpGet("c2cHistory")]
        public async Task<object> Index()
        {
            return (await this.binanceService.GetInvestedAmountAsync());
            //return (await this.binanceConnector.GetC2CHistoryAsync(Side.BUY));
        }

        [HttpGet("GetInvestedAmount")]
        public async Task<object> GetInvestedAmount()
        {
            return await this.cache.GetOrAddAsync("BINANCE_INVESTED_AMOUNT", async () =>
            {
                return await this.binanceService.GetInvestedAmountAsync();
            }, TimeSpan.FromHours(1));
        }
    }
}
