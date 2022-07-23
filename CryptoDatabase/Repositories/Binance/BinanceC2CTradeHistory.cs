using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Binance
{
    [BsonCollection(nameof(BinanceC2CTradeHistory))]
    public class BinanceC2CTradeHistory : Document
    {
        public string? UserId { get; set; }
        public List<BinanceC2CTradeHistoryData>? Data { get; set; }
        public int? Total { get; set; }
       
    }

    public class BinanceC2CTradeHistoryData
    {
        public string? OrderNumber { get; set; }
        public string? AdvNo { get; set; }
        public string? TradeType { get; set; }
        public string? Asset { get; set; }
        public string? Fiat { get; set; }
        public string? FiatSymbol { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? UnitPrice { get; set; }
        public string? OrderStatus { get; set; }
        public long? CreateTime { get; set; }
        public string? Commission { get; set; }
        public string? CounterPartNickName { get; set; }
        public string? AdvertisementRole { get; set; }
    }
}
