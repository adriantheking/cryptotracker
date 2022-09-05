using Newtonsoft.Json;

namespace Models.Connectors.Binance
{
    public class BinancePriceTickerModel
    {
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }
        [JsonProperty("price")]
        public float? Price { get; set; }
    }
}
