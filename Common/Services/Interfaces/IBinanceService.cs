using Binance.Spot.Models;
using Models.Connectors.Binance;

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

        /// <summary>
        /// Returns total invested amount from all sources
        /// </summary>
        /// <returns></returns>
        Task<List<InvestedAmountModel>> GetInvestedAmountAsync();
    }
}
