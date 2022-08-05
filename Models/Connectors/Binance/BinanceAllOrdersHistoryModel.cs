using Newtonsoft.Json;

namespace Models.Connectors.Binance
{
    public class BinanceAllOrdersHistoryModel
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public long? OrderId { get; set; }

        [JsonProperty("orderListId")]
        public long? OrderListId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("price")]
        public string? Price { get; set; }

        [JsonProperty("origQty")]
        public decimal? OrigQty { get; set; }

        [JsonProperty("executedQty")]
        public decimal? ExecutedQty { get; set; }

        [JsonProperty("cummulativeQuoteQty")]
        public decimal? CummulativeQuoteQty { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("stopPrice")]
        public decimal? StopPrice { get; set; }

        [JsonProperty("icebergQty")]
        public decimal? IcebergQty { get; set; }

        [JsonProperty("time")]
        public long? Time { get; set; }

        [JsonProperty("updateTime")]
        public long? UpdateTime { get; set; }

        [JsonProperty("isWorking")]
        public bool? IsWorking { get; set; }

        [JsonProperty("origQuoteOrderQty")]
        public decimal? OrigQuoteOrderQty { get; set; }
    }


}
