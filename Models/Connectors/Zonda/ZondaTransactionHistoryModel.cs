using Newtonsoft.Json;

namespace Models.Connectors.Zonda
{
    public class ZondaTransactionHistoryModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("totalRows")]
        public string TotalRows { get; set; }

        [JsonProperty("items")]
        public List<ZondaTransactionHistoryItemModel>? Items { get; set; }

        [JsonProperty("query")]
        public ZondaTransactionHistoryQueryModel? Query { get; set; }

        [JsonProperty("nextPageCursor")]
        public string NextPageCursor { get; set; }
    }

    public class ZondaTransactionHistoryItemModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("rate")]
        public string Rate { get; set; }

        [JsonProperty("initializedBy")]
        public string InitializedBy { get; set; }

        [JsonProperty("wasTaker")]
        public bool WasTaker { get; set; }

        [JsonProperty("userAction")]
        public string UserAction { get; set; }

        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        [JsonProperty("commissionValue")]
        public string CommissionValue { get; set; }
    }

    public class ZondaTransactionHistoryQueryModel
    {
        [JsonProperty("markets")]
        public List<object> Markets { get; set; }

        [JsonProperty("limit")]
        public List<object> Limit { get; set; }

        [JsonProperty("offset")]
        public List<object> Offset { get; set; }

        [JsonProperty("fromTime")]
        public List<object> FromTime { get; set; }

        [JsonProperty("toTime")]
        public List<object> ToTime { get; set; }

        [JsonProperty("userId")]
        public List<object> UserId { get; set; }

        [JsonProperty("offerId")]
        public List<object> OfferId { get; set; }

        [JsonProperty("initializedBy")]
        public List<object> InitializedBy { get; set; }

        [JsonProperty("rateFrom")]
        public List<object> RateFrom { get; set; }

        [JsonProperty("rateTo")]
        public List<object> RateTo { get; set; }

        [JsonProperty("userAction")]
        public List<object> UserAction { get; set; }

        [JsonProperty("nextPageCursor")]
        public List<string> NextPageCursor { get; set; }
    }
}
