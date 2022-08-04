using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Binance
{
    [BsonCollection(nameof(BinanceOrdersHistory))]
    public class BinanceOrdersHistory : Document
    {
        public string? UserId { get; set; }
        public List<BinanceAllOrdersHistory>? History { get; set; }
        public int? Total { get; set; }
    }
    public class BinanceAllOrdersHistory
    {
        public string Symbol { get; set; }
        public long OrderId { get; set; }
        public long OrderListId { get; set; }
        public string ClientOrderId { get; set; }
        public decimal? Price { get; set; }
        public decimal? OrigQty { get; set; }
        public decimal? ExecutedQty { get; set; }
        public decimal? CummulativeQuoteQty { get; set; }
        public string Status { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public decimal? StopPrice { get; set; }
        public decimal? IcebergQty { get; set; }
        public long? Time { get; set; }
        public long? UpdateTime { get; set; }
        public bool? IsWorking { get; set; }
        public decimal? OrigQuoteOrderQty { get; set; }
    }
}
