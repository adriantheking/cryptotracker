using Newtonsoft.Json;

namespace Models.Connectors.Zonda
{
    public class ZondaTransactionHistoryModel
    {
        [JsonProperty("totalRows")]
        public string TotalRows { get; set; }
        [JsonProperty("items")]
        public List<ZondaTransactionHistoryItemModel>? Items { get; set; }
        [JsonProperty("nextPageCursor")]
        public string? NextPageCursor { get; set; }
    }

    public class ZondaTransactionHistoryItemModel
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("market")]
        public string? Market { get; set; }
        [JsonProperty("time")]
        public string? Time { get; set; }
        [JsonProperty("amount")]
        public decimal? Amount { get; set; }
        [JsonProperty("rate")]
        public decimal? Rate { get; set; }
        [JsonProperty("initializedBy")]
        public string? InitializedBy { get; set; }
        [JsonProperty("wasTaker")]
        public bool? WasTaker { get; set; }
        [JsonProperty("userAction")]
        public string? UserAction { get; set; }
        [JsonProperty("offerId")]
        public string? OfferId { get; set; }
        [JsonProperty("comissionValue")]
        public decimal? CommissionValue { get; set; }
    }
}
