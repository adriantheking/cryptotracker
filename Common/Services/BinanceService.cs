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
        private readonly IMongoRepository<BinanceOrdersHistory> ordersHistoryRepository;

        public BinanceService(ILogger<BinanceService> logger,
            IMapper mapper,
            IBinance binance,
            IMongoRepository<CryptoDatabase.Repositories.Binance.BinanceC2CTradeHistory> c2cRepository,
            IMongoRepository<Wallet> walletRepository,
            IMongoRepository<BinanceOrdersHistory> ordersHistoryRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.binance = binance;
            c2CRepository = c2cRepository;
            this.walletRepository = walletRepository;
            this.ordersHistoryRepository = ordersHistoryRepository;
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

        public async Task<BinanceOrdersHistory> GetSpotOrdersHistoryAsync(List<string> symbols, int yearsToRead = 2)
        {
            var userId = "1111"; //TODO: handle it

            BinanceOrdersHistory binanceOrdersHistory = new BinanceOrdersHistory();
            binanceOrdersHistory.UserId = userId;
            binanceOrdersHistory.History = new List<BinanceOrderHistory>();
            foreach (var symbol in symbols)
            {
                List<BinanceModel.BinanceAllOrdersHistoryModel> history = null;
                try
                {
                    history = await binance.GetOrdersListAsyc(symbol: symbol);
                    if (history != null && history.Any())
                    {
                        var items = mapper.Map<List<BinanceModel.BinanceAllOrdersHistoryModel>, List<BinanceAllOrdersHistory>>(history); //mapper
                        var binanceOrderHistory = new BinanceOrderHistory();
                        binanceOrderHistory.Symbol = symbol;
                        binanceOrderHistory.Data = items;
                        binanceOrdersHistory.History.Add(binanceOrderHistory); //add new items to main object
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex?.Message, ex?.InnerException);
                    throw;
                }
            }

            return binanceOrdersHistory;
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


        public async Task<Wallet> SyncWalletAsync(List<string> symbols, int yearsToRead = 2, bool saveToDb = true)
        {
            var userId = "1111"; //TODO: handle it
            var c2cHistory = await GetC2CTradeHistoryAsync(Side.BUY, yearsToRead);
            var investedAmount = await GetInvestedAmountAsync(c2cHistory);
            var ordersHistory = await GetSpotOrdersHistoryAsync(symbols);
            bool isNewWallet = false;
            bool isNewTradeHistory = false;
            bool isNewOrdersHistory = false;

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

            var allOrders = await ordersHistoryRepository.FindOneAsync(x => x.UserId.Equals(userId));
            if (allOrders == null || string.IsNullOrEmpty(allOrders.UserId))
            {
                isNewOrdersHistory = true;
                allOrders = new BinanceOrdersHistory();
                allOrders.UserId = userId;
                allOrders.History = new List<BinanceOrderHistory>();
                allOrders.Total = 0;
            }
            allOrders.History.AddRange(ordersHistory?.History);
            allOrders.Total += ordersHistory?.History?.Count();

            if (saveToDb)
                if (!isNewWallet)
                    await walletRepository.ReplaceOneAsync(wallet);
                else
                    await walletRepository.InsertOneAsync(wallet);

            if (!isNewTradeHistory)
                await c2CRepository.ReplaceOneAsync(c2cTradeHistory);
            else
                await c2CRepository.InsertOneAsync(c2cTradeHistory);

            if (!isNewOrdersHistory)
                await ordersHistoryRepository.ReplaceOneAsync(allOrders);
            else
                await ordersHistoryRepository.InsertOneAsync(allOrders);

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

        public async Task<BinanceUserTrades> GetSpotTradesHistoryAsync(List<string> symbols)
        {
            var userId = "1111"; //TODO: handle it

            BinanceUserTrades binanceUserTrades = new BinanceUserTrades();
            binanceUserTrades.UserId = userId;
            binanceUserTrades.Trades = new List<BinanceUserTradeSymbolInfo>();

            foreach (var symbol in symbols)
            {
                BinanceUserTradeSymbolInfo userTradeSymbolInfo = new BinanceUserTradeSymbolInfo();
                userTradeSymbolInfo.Symbol = symbol;
                try
                {
                    var trades = await binance.GetTradesListAsync(symbol);
                    if (trades != null && trades.Any())
                    {
                        var items = mapper.Map<List<BinanceModel.BinanceTradesHistoryModel>, List<BinanceUserTrade>>(trades);
                        userTradeSymbolInfo.Data = items;
                        binanceUserTrades.Trades.Add(userTradeSymbolInfo);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex?.Message, ex?.InnerException);
                    throw;
                }
            }

            return binanceUserTrades;
        }
    }
}
