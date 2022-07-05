namespace CryptoCommon.Options
{
    public class BinanceConnectorOptions
    {
        public static string SectionName = "BinanceConnectorOptions";
        public string? BaseUrl { get; set; }
        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }
    }
}
