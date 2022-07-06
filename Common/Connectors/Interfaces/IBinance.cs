using Binance.Spot.Models;
using Models.Connectors.Binance;

namespace CryptoCommon.Connectors.Interfaces
{
    public interface IBinance
    {
        /// <summary>
        /// Returns full history of C2C transactions if no timestamps are set
        /// !!!!!!! IMPORTANT !!!!!! difference between startTimestamp and endTimestamp can be maximum 30 days
        /// </summary>
        /// <param name="side">BUY or SELL. Default: BUY</param>
        /// <param name="startTimestamp">Timestamp where api should start reading. Timestamp NEED TO BE AS UNIX FORMAT ((DateTimeOffset)d).ToUnix</param>
        /// <param name="endTimestamp">Timestamp where api should end reading. Timestamp NEED TO BE AS UNIX FORMAT</param>
        /// <param name="page">Page no</param>
        /// <param name="rows">Rows per page</param>
        /// <param name="recvWindow">check binance docs</param>
        /// <returns></returns>
        Task<BinanceC2CTradeHistory> GetC2CHistoryAsync(Side side, long? startTimestamp = null, long? endTimestamp = null, int? page = null, int? rows = null, long? recvWindow = null);
    }
}
