namespace Models.Services
{
    public class InvestedModel
    {
        public string? Source { get; set; }
        public List<InvestedAmount>? Invested { get; set; }
    }

    public class InvestedAmount
    {
        public string? Fiat { get; set; }
        public decimal? Amount { get; set; }
    }
}
