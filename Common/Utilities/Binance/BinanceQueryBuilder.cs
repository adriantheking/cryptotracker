using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CryptoCommon.Utilities.Binance
{
    public static class BinanceQueryBuilder
    {
        public static string BuildBinanceQuery(this Dictionary<string, string> d)
        {
            StringBuilder output = new StringBuilder("?");
            var convertedParams = HttpUtility.UrlEncode(
                string.Join("&",
                d.Select(param => string.Format("{0}={1}", param.Key, param.Value))
                ));
            output.Append(convertedParams);

            return output.ToString();
        }
    }
}
