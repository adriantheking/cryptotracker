using Binance.Spot.Models;
using CryptoDatabase.Repositories;
using CryptoDatabase.Repositories.Binance;

namespace CryptoCommon.Services.Interfaces
{
    public interface IBinanceService
    {
        /// <summary>
        /// Returns full history from C2C trades.
        /// </summary>
        /// <param name="side">BUY or SELL</param>
        /// <param name="yearsToRead">Value will be substracted from current year</param>
        /// <returns></returns>
        Task<BinanceC2CTradeHistory> GetC2CTradeHistoryAsync(Side side, int yearsToRead = 2);
        //Task<List<BinanceTradeListResponseModel>> Get
        /// <summary>
        /// Returns total invested amount from all sources
        /// </summary>
        /// <returns></returns>
        Task<List<InvestedAmountWallet>> GetInvestedAmountAsync(BinanceC2CTradeHistory c2cHistory);
        /// <summary>
        /// Synchronize Binance wallet including:
        /// - C2C Trade history list
        /// - Total Invested Amount of fiats
        /// </summary>
        /// <param name="yearsToRead">How many years to read data</param>
        /// <returns></returns>
        Task<Wallet> SyncWalletAsync(int yearsToRead = 2, bool saveToDb = true);
    }
}
