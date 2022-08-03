using CryptoDatabase.Repositories;
using CryptoDatabase.Repositories.Zonda;
using Models.Connectors.Zonda;

namespace CryptoCommon.Services.Interfaces
{
    public interface IZondaService
    {
        /// <summary>
        /// Method is returning transaction history from Zonda exchange
        /// </summary>
        /// <returns></returns>
        Task<ZondaTransactionHistoryModel> GetTransactionsAsync();
        Task<ZondaOperationHistory> GetOperationsAsync(bool forceSync = false, string[]? types = null, string[]? accountType = null, string[]? balanceCurrencies = null);
        Task<InvestedAmountWallet?> GetInvestedAmountAsync(string fiat = "PLN", bool forceSync = false);
        /// <summary>
        /// Returns list of all wallets with balance
        /// </summary>
        /// <returns></returns>
        Task<List<ZondaBalancesWalletsModel?>> GetWallets();
        Task<List<ZondaCryptoBalanceModel>> GetCryptoBalancesAsync(string[]? currencies);
        Task<Wallet> SyncWalletAsync(bool saveToDb = true);
    }
}
