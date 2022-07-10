using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories
{
    [BsonCollection("wallet")]
    public class Wallet : Document
    {
        public string? UserId { get; set; }
        public List<InvestedAmountWallet>? Invested { get; set; }

    }
    public class InvestedAmountWallet
    {
        public string? Fiat { get; set; }
        public decimal? Value { get; set; }
        public string? Source { get; set; }
    }
}
