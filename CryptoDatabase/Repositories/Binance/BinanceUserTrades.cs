using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Binance
{
    [BsonCollection(nameof(BinanceUserTrades))]
    public class BinanceUserTrades : Document
    {
        public string? UserId { get; set; }
        public List<BinanceUserTradeSymbolInfo> Trades { get; set; }
    }

    public class BinanceUserTradeSymbolInfo {
        public string? Symbol { get; set; }
        public List<BinanceUserTrade> Data { get; set; }
    }

    public class BinanceUserTrade
    {
        public string? Symbol { get; set; }
        public long? Id { get; set; }
        public long? OrderId { get; set; }
        public long? OrderListId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Qty { get; set; }
        public decimal? QuoteQty { get; set; }
        public decimal? Commission { get; set; }
        public string? CommissionAsset { get; set; }
        public long? Time { get; set; }
        public bool? IsBuyer { get; set; }
        public bool? IsMaker { get; set; }
        public bool? IsBestMatch { get; set; }
    }
}
