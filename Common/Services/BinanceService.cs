using Binance.Spot.Models;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using CryptoCommon.Utilities.Binance;
using Microsoft.Extensions.Logging;
using Models.Connectors.Binance;

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

        public async Task<List<InvestedAmountModel>> GetInvestedAmountAsync()
        {
            List<InvestedAmountModel> investedAmountModel = new List<InvestedAmountModel>();
            //C2C Investment
            var totalC2CInvestment = await GetC2CTradeHistoryAsync(Side.BUY);
            var sumOfC2CInvestment = CalculateFromC2CRecords(totalC2CInvestment);
            AddInvestments(sumOfC2CInvestment, ref investedAmountModel);

            return investedAmountModel;
        }

        /// <summary>
        /// Calculating value for each supported FIAT record from BinanceSupportedConsts.Fiats class 
        /// </summary>
        /// <param name="history"></param>
        /// <returns>List of fiats with invested amount</returns>
        private List<InvestedAmountModel> CalculateFromC2CRecords(BinanceC2CTradeHistory history)
        {
            List<InvestedAmountModel> investedAmount = new List<InvestedAmountModel>();
            BinanceSupportedConsts.Fiats.ForEach(fiat =>
            {
                InvestedAmountModel record = new InvestedAmountModel();
                record.Fiat = fiat.ToUpper();
                record.Value = history.Data.Where(x => x.Fiat.ToUpper().Contains(fiat.ToUpper())).ToList().Sum(x => x.TotalPrice).GetValueOrDefault();//get records for current fiat record                

                investedAmount.Add(record);
            });

            return investedAmount;
        }

        /// <summary>
        /// Adding new data to existing source
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<InvestedAmountModel> AddInvestments(List<InvestedAmountModel> newData, ref List<InvestedAmountModel> source) 
        {
            foreach(var record in newData) { 
                if(source.Any(s => s.Fiat.ToUpper().Equals(record.Fiat.ToUpper())))
                {
                    source.Where(s => s.Fiat.ToUpper().Equals(record.Fiat.ToUpper())).FirstOrDefault().Value += record.Value;
                }
                else
                {
                    source.Add(record);
                }
            };

            return source;
        }
    }
}
