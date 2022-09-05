using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Binance
{
    [BsonCollection(nameof(BinanceTickers))]
    public class BinanceTickers : Document
    {
        public List<BinancePriceTicker> Tickers { get; set; }
    }

    public class BinancePriceTicker
    {
        public string? Symbol { get; set; }
        public float? Price { get; set; }
    }
}
