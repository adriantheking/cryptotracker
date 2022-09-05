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
        Task<BinanceC2CTradeHistory> GetC2CTradeHistoryAsync(Side side, int yearsToRead = 2, bool forceSync = false);
        /// <summary>
        /// Get spot history
        /// </summary>
        /// <param name="yearsToRead">Years to read. If startdate and endtime are not set it will use that property.</param>
        /// <returns></returns>
        Task<BinanceOrdersHistory> GetSpotOrdersHistoryAsync(List<string> symbols, bool forceSync = false);

        Task<BinanceUserTrades> GetSpotTradesHistoryAsync(List<string> symbols, bool forceSync = false);
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
        Task<Wallet> SyncWalletAsync(List<string> symbols, int yearsToRead = 2, bool saveToDb = true);
        Task<CoinInfoWallet> GetCoinInfoAsync(string symbol, bool forceSync = false);
        Task<List<CoinInfoWallet>> GetCoinInfoAsync(List<string> symbol, bool forceSync = false);
        /// <summary>
        /// Returns list of supported stable coins with should be included in trades orders etc
        /// </summary>
        /// <returns></returns>
        Task<List<BinanceSupportedStables>> GetSupportedStablesAsync();
        /// <summary>
        /// Returns list of supported coins what should be synchronized and everything
        /// </summary>
        /// <returns></returns>
        Task<List<BinanceSupportedCoins>> GetSupportedCoinsAsync();

        /// <summary>
        /// returns final coins supported by binance in format: CRYPTOSTABLE: BTCUSDT, BTCBUSDT, ETHUSDT etc...
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetSupportedCombinationCoinsAsync();
    }
}
