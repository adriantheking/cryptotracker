using Models.Connectors.Zonda;
using RestSharp;

namespace CryptoCommon.Connectors.Interfaces
{
    public interface IZonda
    {
        /// <summary>
        /// Method returns transactions related to account
        /// </summary>
        /// <returns></returns>
        public Task<ZondaTransactionHistoryModel?> GetTransactionsAsync();
        /// <summary>
        /// Returns list of operations related with account
        /// </summary>
        /// <param name="types">type of operation ref:https://docs.zonda.exchange/reference/historia-operacji</param>
        /// <param name="sort">desc or asc</param>
        /// <returns></returns>
        public Task<ZondaOperationHistoryModel?> GetOperationsAsync(string[]? types = null, string[]? balanceCurrencies = null, string[]? balanceTypes = null, string sort = "DESC");
        public Task<ZondaMarketStatsModel?> GetMarketStatsAsync(string market);
        /// <summary>
        /// Returns list of all available wallets with ballance
        /// </summary>
        /// <returns></returns>
        public Task<ZondaBalancesModel?> GetWalletsAsync();
    }
}
