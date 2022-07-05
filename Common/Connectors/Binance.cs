using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Options;
using CryptoCommon.Utilities.Binance;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CryptoCommon.Connectors
{
    public class Binance : IBinance
    {
        private readonly ILogger<Binance> logger;
        private readonly BinanceConnectorOptions options;
        private readonly object _sync = new object();

        public Binance(ILogger<Binance> logger,
            IOptions<BinanceConnectorOptions> options)
        {
            this.logger = logger;
            this.options = options?.Value ?? new BinanceConnectorOptions();
            Dictionary<string, string> settings = new Dictionary<string, string>();
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
