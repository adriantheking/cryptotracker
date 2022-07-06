using Binance.Spot.Models;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Connectors.Binance;
using CryptoCommon.Utilities;

namespace CryptoCommon.Services
{
    public class BinanceService : IBinanceService
    {
        private readonly ILogger<BinanceService> logger;
        private readonly IBinance binance;

        public BinanceService(ILogger<BinanceService> logger,
            IBinance binance)
        {
            this.logger = logger;
            this.binance = binance;
        }

        public async Task<BinanceC2CTradeHistory> GetC2CTradeHistoryAsync(Side side, int yearsToRead = 2)
        {
            var stopYear = DateTime.Now.AddYears(-yearsToRead); //subsctract years
            var endDay = DateTime.UtcNow;
            var startDay = endDay.AddDays(-30);

            var endTimespan = ((DateTimeOffset)endDay).ToUnixTimeMilliseconds(); //ends today
            var startTimespan = ((DateTimeOffset)startDay).ToUnixTimeMilliseconds(); //start 30days before
            BinanceC2CTradeHistory? binanceC2CTradeHistory = null;

            do
            {
                if (binanceC2CTradeHistory == null)
                {
                    try
                    {
                        binanceC2CTradeHistory = await binance.GetC2CHistoryAsync(side, startTimespan, endTimespan);
                        endDay = startDay;
                        startDay = startDay.AddDays(-30);
                        endTimespan = ((DateTimeOffset)endDay).ToUnixTimeMilliseconds();
                        startTimespan = ((DateTimeOffset)startDay).ToUnixTimeMilliseconds();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex?.Message, ex?.InnerException);
                        throw;
                    }

                }
                else
                {
                    try
                    {
                        var history = await binance.GetC2CHistoryAsync(side, startTimespan, endTimespan);
                        if(history != null && history.Data != null && history.Data.Any())
                        {
                            binanceC2CTradeHistory?.Data?.AddRange(history.Data); //append results to main object
                            binanceC2CTradeHistory.Total += history.Total;
                        }
                        //increase counters
                        endDay = startDay;
                        startDay = startDay.AddDays(-30);
                        endTimespan = ((DateTimeOffset)endDay).ToUnixTimeMilliseconds();
                        startTimespan = ((DateTimeOffset)startDay).ToUnixTimeMilliseconds();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex?.Message, ex?.InnerException);
                        throw;
                    }
                }

            } while (DateTime.Compare(startDay, stopYear) > 0);
            return binanceC2CTradeHistory ?? new BinanceC2CTradeHistory();
        }
    }
}
