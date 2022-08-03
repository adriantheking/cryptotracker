using AutoMapper;
using Binance.Spot.Models;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using CryptoCommon.Utilities.Binance;
using CryptoDatabase.Repositories;
using CryptoDatabase.Repositories.Binance;
using CryptoDatabase.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using BinanceModel = Models.Connectors.Binance;

namespace CryptoCommon.Services
{
    public class BinanceService : IBinanceService
    {
        private readonly ILogger<BinanceService> logger;
        private readonly IMapper mapper;
        private readonly IBinance binance;
        private readonly IMongoRepository<CryptoDatabase.Repositories.Binance.BinanceC2CTradeHistory> c2CRepository;
        private readonly IMongoRepository<Wallet> walletRepository;

        public BinanceService(ILogger<BinanceService> logger,
            IMapper mapper,
            IBinance binance,
            IMongoRepository<CryptoDatabase.Repositories.Binance.BinanceC2CTradeHistory> c2cRepository,
            IMongoRepository<Wallet> walletRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.binance = binance;
            c2CRepository = c2cRepository;
            this.walletRepository = walletRepository;
        }

        public async Task<BinanceC2CTradeHistory> GetC2CTradeHistoryAsync(Side side, int yearsToRead = 2)
        {
            var userId = "1111"; //TODO: handle it
            var stopYear = DateTime.Now.AddYears(-yearsToRead); //subsctract years
            var endDay = DateTime.UtcNow;
            var startDay = endDay.AddDays(-30);

            var endTimespan = ((DateTimeOffset)endDay).ToUnixTimeMilliseconds(); //ends today
            var startTimespan = ((DateTimeOffset)startDay).ToUnixTimeMilliseconds(); //start 30days before
            BinanceC2CTradeHistory binanceC2CTradeHistory = new BinanceC2CTradeHistory();
            binanceC2CTradeHistory.UserId = userId;
            binanceC2CTradeHistory.Data = new List<BinanceC2CTradeHistoryData>();
            binanceC2CTradeHistory.Total = 0;
            do
            {
                BinanceModel.BinanceC2CTradeHistory history = null;
                try
                {
                    history = await binance.GetC2CHistoryAsync(side, startTimespan, endTimespan);
                    if (history != null && history.Data != null && history.Data.Any())
                    {
                        binanceC2CTradeHistory.Total += history.Total;
                        binanceC2CTradeHistory?.Data?.AddRange(mapper.Map<List<BinanceModel.BinanceC2CTradeHistoryData>, List<BinanceC2CTradeHistoryData>>(history.Data)); //append results to main object
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex?.Message, ex?.InnerException);
                    throw;
                }

                //increase counters
                endDay = startDay;
                startDay = startDay.AddDays(-30);
                endTimespan = ((DateTimeOffset)endDay).ToUnixTimeMilliseconds();
                startTimespan = ((DateTimeOffset)startDay).ToUnixTimeMilliseconds();

            } while (DateTime.Compare(startDay, stopYear) > 0);
            return binanceC2CTradeHistory ?? new BinanceC2CTradeHistory();
        }

        public async Task<List<InvestedAmountWallet>> GetInvestedAmountAsync(BinanceC2CTradeHistory c2cHistory)
        {
            List<InvestedAmountWallet> investedAmountModel = new List<InvestedAmountWallet>();
            //C2C Investment
            var totalC2CInvestment = c2cHistory;
            var sumOfC2CInvestment = CalculateFromC2CRecords(totalC2CInvestment);
            AddInvestments(sumOfC2CInvestment, ref investedAmountModel);

            return investedAmountModel;
        }


        public async Task<Wallet> SyncWalletAsync(int yearsToRead = 2, bool saveToDb = true)
        {
            var userId = "1111"; //TODO: handle it
            var c2cHistory = await GetC2CTradeHistoryAsync(Side.BUY, yearsToRead);
            var investedAmount = await GetInvestedAmountAsync(c2cHistory);
            bool isNewWallet = false;
            bool isNewTradeHistory = false;

            var wallet = await walletRepository.FindOneAsync(x => x.UserId.Equals(userId));
            if (wallet == null || string.IsNullOrEmpty(wallet.UserId))
            {
                isNewWallet = true;
                wallet = new Wallet();
                wallet.UserId = userId;
            }
            wallet.Invested = investedAmount;

            var c2cTradeHistory = await c2CRepository.FindOneAsync(x => x.UserId.Equals(userId));
            if (c2cTradeHistory == null || string.IsNullOrEmpty(c2cTradeHistory.UserId))
            {
                isNewTradeHistory = true;
                c2cTradeHistory = new BinanceC2CTradeHistory();
                c2cTradeHistory.UserId = userId;
            }
            c2cTradeHistory.Data = c2cHistory.Data;
            c2cTradeHistory.Total = c2cHistory.Total;


            if (saveToDb)
                if (!isNewWallet)
                    await walletRepository.ReplaceOneAsync(wallet);
                else
                    await walletRepository.InsertOneAsync(wallet);

            if (!isNewTradeHistory)
                await c2CRepository.ReplaceOneAsync(c2cTradeHistory);
            else
                await c2CRepository.InsertOneAsync(c2cTradeHistory);

            return wallet;
        }

        /// <summary>
        /// Calculating value for each supported FIAT record from BinanceSupportedConsts.Fiats class 
        /// </summary>
        /// <param name="history"></param>
        /// <returns>List of fiats with invested amount</returns>
        private List<InvestedAmountWallet> CalculateFromC2CRecords(BinanceC2CTradeHistory history)
        {
            List<InvestedAmountWallet> investedAmount = new List<InvestedAmountWallet>();
            BinanceSupportedConsts.Fiats.ForEach(fiat =>
            {
                InvestedAmountWallet record = new InvestedAmountWallet();
                record.Fiat = fiat.ToUpper();
                record.Value = history.Data.Where(x => x.Fiat.ToUpper().Contains(fiat.ToUpper())).ToList().Sum(x => x.TotalPrice).GetValueOrDefault();//get records for current fiat record                
                record.Source = nameof(IBinance);
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
        private List<InvestedAmountWallet> AddInvestments(List<InvestedAmountWallet> newData, ref List<InvestedAmountWallet> source)
        {
            foreach (var record in newData)
            {
                if (source.Any(s => s.Fiat.ToUpper().Equals(record.Fiat.ToUpper())))
                {
                    var obj = source.Where(s => s.Fiat.ToUpper().Equals(record.Fiat.ToUpper())).FirstOrDefault();
                    obj.Value += record.Value;
                    obj.Source = nameof(IBinance);
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
