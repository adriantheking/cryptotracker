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
        /// Fill wallet data in database with required data in dashboard
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<MongoWallet> FillWalletAsync(string userId);
        Task<TransactionHistoryModel> GetTransactionsAsync();
    }
}
