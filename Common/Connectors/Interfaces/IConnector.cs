using RestSharp;

namespace Common.Connectors.Interfaces
{
    public interface IConnector
    {
        public string GetTransactionsAsync();
        public RestClientOptions? SetRestOptions();
    }
}
