using Models.Connectors.Zonda;
using RestSharp;

namespace Common.Connectors.Interfaces
{
    public interface IConnector
    {
        /// <summary>
        /// Method returns transactions related to account
        /// </summary>
        /// <returns></returns>
        public Task<ZondaTransactionHistoryModel> GetTransactionsAsync();
        public RestClientOptions? SetRestOptions();
    }
}
