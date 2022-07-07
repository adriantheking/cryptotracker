using Binance.Spot.Models;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Connectors.Binance;
using CryptoCommon.Utilities;
using CryptoCommon.Utilities.Binance;

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
                BinanceC2CTradeHistory history = null;
                try
                {
                    history = await binance.GetC2CHistoryAsync(side, startTimespan, endTimespan);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex?.Message, ex?.InnerException);
                    throw;
                }
                if (binanceC2CTradeHistory == null)
                {
                    binanceC2CTradeHistory = history;
                }
                else
                {
                    if (history != null && history.Data != null && history.Data.Any())
                    {
                        binanceC2CTradeHistory?.Data?.AddRange(history.Data); //append results to main object
                        binanceC2CTradeHistory.Total += history.Total;
                    }               
                }
                //increase counters
                endDay = startDay;
                startDay = startDay.AddDays(-30);
                endTimespan = ((DateTimeOffset)endDay).ToUnixTimeMilliseconds();
                startTimespan = ((DateTimeOffset)startDay).ToUnixTimeMilliseconds();

            } while (DateTime.Compare(startDay, stopYear) > 0);
            return binanceC2CTradeHistory ?? new BinanceC2CTradeHistory();
        }

        public async Task<Dictionary<string, decimal>> GetInvestedAmountAsync()
        {
            //C2C Investment
            var totalC2CInvestment = await GetC2CTradeHistoryAsync(Side.BUY);
            var sumOfC2CInvestment = CalculateFromC2CRecords(totalC2CInvestment);

            return sumOfC2CInvestment;
        }

        private Dictionary<string, decimal> CalculateFromC2CRecords(BinanceC2CTradeHistory history)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            BinanceSupportedConsts.Fiats.ForEach(fiat =>
            {
                //get records for current fiat record
                result.Add(fiat.ToUpper(), history.Data.Where(x => x.Fiat.ToUpper().Contains(fiat.ToUpper())).ToList().Sum(x => x.TotalPrice).GetValueOrDefault());
            });

            return result;
        }
    }
}
