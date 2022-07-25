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
            return Ok();
        }

        [HttpGet("GetInvestedAmount")]
        public async Task<object> GetInvestedAmount()
        {
            return Ok();
        }
    }
}
