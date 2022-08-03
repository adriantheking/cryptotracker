using Models.Services;
using MongoWallet = CryptoDatabase.Repositories.Wallet;

namespace CryptoCommon.Services.Interfaces
{
    public interface IDashboardService
    {
        /// <summary>
        /// Returns Wallet object
        /// </summary>
        /// <returns></returns>
        Task<MongoWallet> GetWalletAsync();
        /// <summary>
        /// Sync wallet data in database with required data in dashboard
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wallet">Wallet to synchronize. If empty new will be created</param>
        /// <returns></returns>
        Task<MongoWallet> SyncWalletAsync(string userId);
        Task<TransactionHistoryModel> GetTransactionsAsync();
    }
}
