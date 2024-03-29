﻿using Newtonsoft.Json;

namespace Models.Connectors.Zonda
{
    public class ZondaOperationHistoryModel
    {
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("items")]
        public List<ZondaOperationHistoryItemModel>? Items { get; set; }

        [JsonProperty("hasNextPage")]
        public bool? HasNextPage { get; set; }

        [JsonProperty("fetchedRows")]
        public int? FetchedRows { get; set; }

        [JsonProperty("limit")]
        public int? Limit { get; set; }

        [JsonProperty("offset")]
        public int? Offset { get; set; }

        [JsonProperty("queryTime")]
        public int? QueryTime { get; set; }

        [JsonProperty("totalTime")]
        public int? TotalTime { get; set; }

        [JsonProperty("errors")]
        public object? Errors { get; set; }
    }

    public class ZondaOperationHistoryItemModel
    {
        [JsonProperty("historyId")]
        public string? HistoryId { get; set; }

        [JsonProperty("balance")]
        public ZondaOperationHistoryBalanceModel? Balance { get; set; }

        [JsonProperty("detailId")]
        public object? DetailId { get; set; }

        [JsonProperty("time")]
        public long? Time { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [JsonProperty("fundsBefore")]
        public ZondaOperationHistoryFundsBeforeModel? FundsBefore { get; set; }

        [JsonProperty("fundsAfter")]
        public ZondaOperationHistoryFundsAfterModel? FundsAfter { get; set; }

        [JsonProperty("change")]
        public ZondaOperationHistoryChangeModel? Change { get; set; }
    }

    public class ZondaOperationHistoryBalanceModel
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("currency")]
        public string? Currency { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }

    public class ZondaOperationHistoryChangeModel
    {
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        [JsonProperty("available")]
        public decimal? Available { get; set; }

        [JsonProperty("locked")]
        public decimal? Locked { get; set; }
    }

    public class ZondaOperationHistoryFundsAfterModel
    {
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        [JsonProperty("available")]
        public decimal? Available { get; set; }

        [JsonProperty("locked")]
        public decimal? Locked { get; set; }
    }

    public class ZondaOperationHistoryFundsBeforeModel
    {
        [JsonProperty("total")]
        public object? Total { get; set; }

        [JsonProperty("available")]
        public object? Available { get; set; }

        [JsonProperty("locked")]
        public object? Locked { get; set; }
    }


}
