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
        private readonly IMongoRepository<BinanceUserTrades> userTradesRepository;

        public BinanceService(ILogger<BinanceService> logger,
            IMapper mapper,
            IBinance binance,
            IMongoRepository<CryptoDatabase.Repositories.Binance.BinanceC2CTradeHistory> c2cRepository,
            IMongoRepository<Wallet> walletRepository,
            IMongoRepository<BinanceOrdersHistory> ordersHistoryRepository,
            IMongoRepository<BinanceUserTrades> userTradesRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.binance = binance;
            c2CRepository = c2cRepository;
            this.walletRepository = walletRepository;
            this.ordersHistoryRepository = ordersHistoryRepository;
            this.userTradesRepository = userTradesRepository;
        }

        public async Task<BinanceC2CTradeHistory> GetC2CTradeHistoryAsync(Side side, int yearsToRead = 2, bool forceSync = false)
        {
            var userId = "1111"; //TODO: handle it
            if (!forceSync)
            {
                var history = await c2CRepository.FindOneAsync(x => x.UserId.Equals(userId));
                if (history != null)
                    return history;
            }

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

        public async Task<BinanceOrdersHistory> GetSpotOrdersHistoryAsync(List<string> symbols, bool forceSync = false)
        {
            var userId = "1111"; //TODO: handle it
            if (!forceSync)
            {
                var history = await ordersHistoryRepository.FindOneAsync(x => x.UserId.Equals(userId));
                if (history != null)
                    return history;
            }
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
            var c2cHistory = await GetC2CTradeHistoryAsync(Side.BUY, yearsToRead, true);
            var investedAmount = await GetInvestedAmountAsync(c2cHistory);
            var ordersHistory = await GetSpotOrdersHistoryAsync(symbols, true);
            var spotTradesHistory = await GetSpotTradesHistoryAsync(symbols, true);

            bool isNewWallet = false;
            bool isNewTradeHistory = false;
            bool isNewOrdersHistory = false;
            bool isNewSpotTradeHistory = false;

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
            allOrders.History = ordersHistory?.History;
            allOrders.Total += ordersHistory?.History?.Count();

            var spotTrades = await userTradesRepository.FindOneAsync(x => x.UserId.Equals(userId));
            if (spotTrades == null || string.IsNullOrEmpty(spotTrades.UserId))
            {
                isNewSpotTradeHistory = true;
                spotTrades = new BinanceUserTrades();
                spotTrades.UserId = userId;
                spotTrades.Trades = new List<BinanceUserTradeSymbolInfo>();
            }
            spotTrades.Trades = spotTradesHistory.Trades;


            ///UPDATE WALLET ABOUT COINS INFO
            if (spotTrades.Trades != null && spotTrades.Trades.Any())
            {
                var coinfoInfoWallet = await GetCoinInfoAsync(symbols, true); //no need force refresh here all data has been already refreshed
                if(coinfoInfoWallet != null)
                {
                    wallet.Coins = coinfoInfoWallet;
                }
                else
                {
                    wallet.Coins = new List<CoinInfoWallet>();
                }
            }

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

            if (!isNewSpotTradeHistory)
                await userTradesRepository.ReplaceOneAsync(spotTrades);
            else
                await userTradesRepository.InsertOneAsync(spotTrades);

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

        public async Task<BinanceUserTrades> GetSpotTradesHistoryAsync(List<string> symbols, bool forceSync = false)
        {
            var userId = "1111"; //TODO: handle it
            if (!forceSync)
            {
                var history = await userTradesRepository.FindOneAsync(x => x.UserId.Equals(userId));
                if (history != null)
                    return history;
            }
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
        public async Task<CoinInfoWallet> GetCoinInfoAsync(string symbol, bool forceSync = false)
        {
            var userId = "1111"; //TODO: handle it;
            if (!forceSync)
            {
                var wallet = await walletRepository.FindOneAsync(x => x.UserId.Equals(userId));
                if (wallet != null && wallet.Coins != null && wallet.Coins.Any())
                {
                    var coinInfo = wallet.Coins.FirstOrDefault(x => x.Symbol.Equals(symbol));
                    if (coinInfo != null)
                        return coinInfo;
                }
            }
            var trades = await GetSpotTradesHistoryAsync(new List<string> { symbol }, forceSync);
            if (trades != null && trades.Trades != null)
            {
                var symbolTrades = trades.Trades.FirstOrDefault(x => x.Symbol.Equals(symbol));
                if (symbolTrades != null && symbolTrades.Data != null && symbolTrades.Data.Any())
                {
                    var buyerData = symbolTrades.Data.Where(x => x.IsBuyer.HasValue && x.IsBuyer.Value);
                    var sellerData = symbolTrades.Data.Where(x => x.IsBuyer.HasValue && !x.IsBuyer.Value);

                    var buyerCoinQty = buyerData.Sum(x => x.Qty);
                    var buyerQuoteQty = buyerData.Sum(x => x.QuoteQty);
                    var sellerCoinQty = sellerData.Sum(x => x.Qty);
                    var sellerQuoteQty = sellerData.Sum(x => x.QuoteQty);


                    var cointQty = buyerCoinQty - sellerCoinQty; //number of bought
                    var quoteQty = buyerQuoteQty - sellerQuoteQty; //value of spend money

                    return new CoinInfoWallet()
                    {
                        Amount = cointQty,
                        AveragePrice = quoteQty,
                        Symbol = symbol,
                        Source = nameof(IBinance),
                        TotalInvested = quoteQty
                    };
                }
            }
            return new CoinInfoWallet();
        }
        public async Task<List<CoinInfoWallet>> GetCoinInfoAsync(List<string> symbol, bool forceSync = false)
        {
            var userId = "1111"; //TODO: handle it;
            if (!forceSync)
            {
                var wallet = await walletRepository.FindOneAsync(x => x.UserId.Equals(userId));
                if (wallet != null && wallet.Coins != null && wallet.Coins.Any())
                {
                    return wallet.Coins;
                }
            }
            var trades = await GetSpotTradesHistoryAsync(symbol, forceSync);
            var spotOrders = await GetSpotOrdersHistoryAsync(symbol, forceSync);

            List<CoinInfoWallet> coinInfoWallet = new List<CoinInfoWallet>();
            if (trades != null && trades.Trades != null)
            {
                foreach (var symb in symbol)
                {
                    var symbolTrades = trades.Trades.FirstOrDefault(x => x.Symbol.Equals(symb));
                    var symbolOrders = spotOrders.History.FirstOrDefault(x => x.Symbol.Equals(symb));
                    var coinInfo = new CoinInfoWallet();
                    decimal? cointQty = 0;
                    decimal? quoteQty = 0;

                    if (symbolTrades != null && symbolTrades.Data != null && symbolTrades.Data.Any())
                    {
                        var buyerData = symbolTrades.Data.Where(x => x.IsBuyer.HasValue && x.IsBuyer.Value);
                        var sellerData = symbolTrades.Data.Where(x => x.IsBuyer.HasValue && !x.IsBuyer.Value);

                        var buyerCoinQty = buyerData.Sum(x => x.Qty);
                        var buyerQuoteQty = buyerData.Sum(x => x.QuoteQty);
                        var sellerCoinQty = sellerData.Sum(x => x.Qty);
                        var sellerQuoteQty = sellerData.Sum(x => x.QuoteQty);


                         cointQty = buyerCoinQty - sellerCoinQty; //number of bought
                         quoteQty = buyerQuoteQty - sellerQuoteQty; //value of spend money

                        coinInfo = new CoinInfoWallet
                        {
                            Amount = cointQty,
                            AveragePrice = quoteQty / cointQty,
                            Symbol = symb,
                            Source = nameof(IBinance),
                            TotalInvested = quoteQty
                        };
                    }
                    if(symbolOrders != null && symbolOrders.Data != null && symbolOrders.Data.Any())
                    {
                        var buyerData = symbolOrders.Data.Where(x => x.Side == Side.BUY && x.Type == "LIMIT" && x.Status == "FILLED");
                        var sellerData = symbolOrders.Data.Where(x => x.Side == Side.SELL && x.Type == "LIMIT" && x.Status == "FILLED");

                        var buyerCoinQty = buyerData.Sum(x => x.ExecutedQty);
                        var buyerQuoteQty = buyerData.Sum(x => x.CummulativeQuoteQty);
                        var sellerCoinQty = sellerData.Sum(x => x.ExecutedQty);
                        var sellerQuoteQty = sellerData.Sum(x => x.CummulativeQuoteQty);

                        cointQty += buyerCoinQty - sellerCoinQty; //number of bought
                        quoteQty += buyerQuoteQty - sellerQuoteQty; //value of spend money
                        coinInfo.Amount = cointQty;
                        coinInfo.AveragePrice = quoteQty / cointQty;
                    }
                    coinInfoWallet.Add(coinInfo);
                }
            }
            return coinInfoWallet;
        }
    }
}
