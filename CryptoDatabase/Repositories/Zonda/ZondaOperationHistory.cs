using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Zonda
{
    [BsonCollection(nameof(ZondaOperationHistory))]
    public class ZondaOperationHistory : Document
    {
        public string? UserId { get; set; }
        public string? Status { get; set; }

        public ZondaOperationHistoryItem[]? Items { get; set; }

        public int? FetchedRows { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        public object? Errors { get; set; }
    }

    public class ZondaOperationHistoryItem
    {
        public string? HistoryId { get; set; }

        public ZondaOperationHistoryBalance? Balance { get; set; }

        public object? DetailId { get; set; }

        public long? Time { get; set; }

        public string? Type { get; set; }

        public decimal? Value { get; set; }

        public ZondaOperationHistoryFundsBefore? FundsBefore { get; set; }

        public ZondaOperationHistoryFundsAfter? FundsAfter { get; set; }

        public ZondaOperationHistoryChange? Change { get; set; }
    }

    public class ZondaOperationHistoryBalance
    {
        public string? Id { get; set; }

        public string? Currency { get; set; }

        public string? Type { get; set; }

        public string? UserId { get; set; }

        public string? Name { get; set; }
    }

    public class ZondaOperationHistoryChange
    {
        public decimal? Total { get; set; }

        public decimal? Available { get; set; }

        public decimal? Locked { get; set; }
    }

    public class ZondaOperationHistoryFundsAfter
    {
        public decimal? Total { get; set; }

        public decimal? Available { get; set; }

        public decimal? Locked { get; set; }
    }

    public class ZondaOperationHistoryFundsBefore
    {
        public object? Total { get; set; }

        public object? Available { get; set; }

        public object? Locked { get; set; }
    }
}
