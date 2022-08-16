using CryptoDatabase.Attributes;

namespace CryptoDatabase.Repositories
{
    [BsonCollection("wallet")]
    public class Wallet : Document
    {
        public string? UserId { get; set; }
        public List<InvestedAmountWallet>? Invested { get; set; }
        public List<CoinInfoWallet>? Coins { get; set; }

    }
    public class InvestedAmountWallet
    {
        public string? Fiat { get; set; }
        public decimal? Value { get; set; }
        public string? Source { get; set; }
    }
    public class CoinInfoWallet
    {
        public string? Symbol { get; set; }
        public string? Source { get; set; }
        public decimal? AveragePrice { get; set; }
        public decimal? TotalInvested { get; set; }
        public decimal? Amount { get; set; }
    }
}
