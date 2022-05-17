using Models.Connectors.Zonda;

namespace Common.Services.Interfaces
{
    public interface IZondaService
    {
        /// <summary>
        /// Method is returning transaction history from Zonda exchange
        /// </summary>
        /// <returns></returns>
        Task<ZondaTransactionHistoryModel> GetTransactionsAsync();
        Task<ZondaOperationHistoryModel> GetOperationsAsync(string[]? types = null, string[]? accountType = null, string[]? balanceCurrencies = null);
        Task<decimal> GetInvestedAmountAsync();
        /// <summary>
        /// Returns list of all wallets with balance
        /// </summary>
        /// <returns></returns>
        Task<List<ZondaBalancesWalletsModel?>> GetWallets();
        Task<List<ZondaCryptoBalanceModel>> GetCryptoBalancesAsync(string[]? currencies);
    }
}
