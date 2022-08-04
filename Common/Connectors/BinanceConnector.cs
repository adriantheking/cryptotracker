using Binance.Spot;
using Binance.Spot.Models;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Connectors.Binance;
using Newtonsoft.Json;

namespace CryptoCommon.Connectors
{
    //for docs check interface
    //There was problem with Binance name... Binance.Spot dll has the same name
    public class BinanceConnector : IBinance
    {
        private readonly ILogger<BinanceConnector> logger;
        private readonly HttpClient httpClient;
        private readonly BinanceConnectorOptions options;
        private readonly object _sync = new object();

        public BinanceConnector(ILogger<BinanceConnector> logger,
            HttpClient httpClient,
            IOptions<BinanceConnectorOptions> options)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.options = options?.Value ?? new BinanceConnectorOptions();

            this.httpClient.BaseAddress = new Uri(this.options.BaseUrl);
        }

        public async Task<BinanceC2CTradeHistory> GetC2CHistoryAsync(Side side, long? startTimestamp = null, long? endTimestamp = null, int? page = null, int? rows = null, long? recvWindow = null)
        {
            if (side == null)
                side = Side.BUY;

            C2C c2c = new C2C(httpClient, options.BaseUrl, options.PublicKey, options.PrivateKey);
            try
            {
                var result = await c2c.GetC2cTradeHistory(side,
                    startTimestamp: startTimestamp,
                    endTimestamp: endTimestamp,
                    page: page,
                    rows: rows,
                    recvWindow: recvWindow);
                var history = JsonConvert.DeserializeObject<BinanceC2CTradeHistory>(result);

                return history;
            }
            catch (Exception ex)
            {
                logger.LogError(ex?.Message, ex, ex?.InnerException);
                throw;
            }
        }

        public async Task<List<BinanceAllOrdersHistoryModel>> GetOrdersListAsyc(string symbol, long? orderId = null, long? startTime = null, long? endTime = null, long? fromId = null, int? limit = null, long? recvWindow = null)
        {
            var spotAccountTrade = new SpotAccountTrade(httpClient, options.BaseUrl, options.PublicKey, options.PrivateKey);

            try
            {
                var result = await spotAccountTrade.AllOrders(symbol: symbol,
                    orderId: orderId,
                    startTime: startTime,
                    endTime: endTime,
                    limit: limit,
                    recvWindow: recvWindow);
                return JsonConvert.DeserializeObject<List<BinanceAllOrdersHistoryModel>>(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }
        }
    }
}
