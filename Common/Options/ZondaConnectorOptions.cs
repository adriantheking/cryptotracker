namespace CryptoCommon.Options
{
    public class ZondaConnectorOptions
    {
        public static string SectionName = "ZondaConnectorOptions";
        public string? BaseUrl { get; set; }
        public int? TimeOut { get; set; }
        public string? PrivateKey { get; set; }
        public string? PublicKey { get; set; }
    }
}
