using RestSharp;

namespace Common.Connectors.Interfaces
{
    public interface IConnector
    {
        public Task<string> GetTransactionsAsync();
        public RestClientOptions? SetRestOptions();
    }
}
