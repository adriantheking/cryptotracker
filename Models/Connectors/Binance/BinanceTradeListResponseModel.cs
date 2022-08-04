using Newtonsoft.Json;

namespace Models.Connectors.Binance
{
    public class BinanceTradeListResponseModel
    {
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }
        public List<BinanceAllOrdersHistoryModel>? Trades { get; set; }
    }
}
