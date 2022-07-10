namespace CryptoCommon.Options
{
    public class MongoOptions
    {
        public static string SectionName = "Mongo";
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
    }
}
