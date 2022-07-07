using Newtonsoft.Json;

namespace Models.Connectors.Binance
{
    public class BinanceC2CTradeHistory
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<BinanceC2CTradeHistoryData> Data { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class BinanceC2CTradeHistoryData
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("advNo")]
        public string AdvNo { get; set; }

        [JsonProperty("tradeType")]
        public string TradeType { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("fiat")]
        public string Fiat { get; set; }

        [JsonProperty("fiatSymbol")]
        public string FiatSymbol { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("totalPrice")]
        public decimal? TotalPrice { get; set; }

        [JsonProperty("unitPrice")]
        public string UnitPrice { get; set; }

        [JsonProperty("orderStatus")]
        public string OrderStatus { get; set; }

        [JsonProperty("createTime")]
        public long CreateTime { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("counterPartNickName")]
        public string CounterPartNickName { get; set; }

        [JsonProperty("advertisementRole")]
        public string AdvertisementRole { get; set; }
    }
}
