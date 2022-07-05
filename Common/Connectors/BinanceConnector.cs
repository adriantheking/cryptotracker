using Binance.Spot;
using Binance.Spot.Models;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Connectors.Binance;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace CryptoCommon.Connectors
{
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

        private string GetHMAC(string privateKey, string totalParams)
        {
            var privateKeyBytes = Encoding.ASCII.GetBytes(privateKey);
            var totalParamsBytes = Encoding.ASCII.GetBytes(totalParams);
            byte[] hash;

            HMACSHA256 hmac = new HMACSHA256(privateKeyBytes);
            lock (_sync)
            {
                hash = hmac.ComputeHash(totalParamsBytes);
            }
            var output = new StringBuilder();
            foreach (var h in hash)
                output.Append(h.ToString("x2"));
            return output.ToString();
        }
    }
}
