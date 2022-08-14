using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories.Binance
{
    [BsonCollection(nameof(BinanceSupportedStables))]
    public class BinanceSupportedStables : Document
    {
        public string? Name { get; set; }
    }
}
