using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Binance
{
    [BsonCollection(nameof(BinanceSupportedCoins))]
    public class BinanceSupportedCoins : Document
    {
        public string? Name { get; set; }
    }
}
