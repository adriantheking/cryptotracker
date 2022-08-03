using AutoMapper;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using CryptoCommon.Utilities;
using CryptoCommon.Utilities.Zonda;
using CryptoDatabase.Repositories;
using CryptoDatabase.Repositories.Interfaces;
using CryptoDatabase.Repositories.Zonda;
using LazyCache;
using Microsoft.Extensions.Logging;
using Models.Connectors.Zonda;

namespace CryptoCommon.Services
{
    public class ZondaService : IZondaService
    {
        private readonly ILogger<ZondaService> logger;
        private readonly IAppCache cache;
        private readonly IZonda zondaConnector;
        private readonly IMapper mapper;
        private readonly IMongoRepository<Wallet> walletRepository;
        private readonly IMongoRepository<ZondaOperationHistory> operationsHistoryRepository;
        private static readonly string ADD_FUNDS_PROP = "ADD_FUNDS";
        private static readonly string SUBTRACT_FUNDS_PROP = "SUBTRACT_FUNDS";
        private static readonly string WITHDRAWAL_SUBTRACT_FUNDS_PROP = "WITHDRAWAL_SUBTRACT_FUNDS";
        private static readonly string TRANSACTION_POST_INCOME = "TRANSACTION_POST_INCOME";
        private static readonly string TRANSACTION_POST_OUTCOME = "TRANSACTION_POST_OUTCOME";
        private static readonly string WITHDRAWAL_SUBTRACT_FUNDS = "WITHDRAWAL_SUBTRACT_FUNDS";
        private static readonly string TRANSACTION_COMMISSION_OUTCOME = "TRANSACTION_COMMISSION_OUTCOME";
        private static readonly string OPERATIONS_CACHE_KEY = "OPERATIONS_";
        private static readonly string TRANSACTIONS_CACHE_KEY = "TRANSACTIONS_";
        private static readonly string WALLETS_CACHE_KEY = "WALLETS_";
        private static readonly string MARKET_STATS_CACHE_KEY = "MARKETSTATS_";

        public ZondaService(ILogger<ZondaService> logger, IAppCache cache, IZonda zondaConnector,
            IMapper mapper,
            IMongoRepository<Wallet> walletRepository, IMongoRepository<ZondaOperationHistory> operationsHistoryRepository)
        {
            this.logger = logger;
            this.cache = cache;
            this.zondaConnector = zondaConnector;
            this.mapper = mapper;
            this.walletRepository = walletRepository;
            this.operationsHistoryRepository = operationsHistoryRepository;
        }

        public async Task<InvestedAmountWallet?> GetInvestedAmountAsync(string fiat = "PLN", bool forceSync = false)
        {
            var types = new string[] { ADD_FUNDS_PROP, SUBTRACT_FUNDS_PROP, WITHDRAWAL_SUBTRACT_FUNDS_PROP };
            var accountTypes = new string[] { "FIAT" }; //TODO: HANDLE
            var operations = await this.GetOperationsAsync(forceSync: forceSync, types: types, accountType: accountTypes);
            try
            {
                if (operations != null && operations.Items != null)
                {
                    var investedAmount = operations.Items.Where(x => x.Balance.Currency.Equals(fiat.ToUpper())).Sum(x => x.Value).Value; //TODO: HANDLE
                    return new InvestedAmountWallet()
                    {
                        Fiat = fiat.ToUpper(),
                        Value = investedAmount,
                        Source = nameof(IZonda)
                    };
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }

            return null;
        }

        public async Task<ZondaOperationHistory> GetOperationsAsync(bool forceSync = false, string[]? types = null, string[]? accountType = null, string[]? balanceCurrencies = null)
        {
            try
            {
                var userId = "1111"; //TODO: Handle it
                var isNewCollection = false;
                var history = await operationsHistoryRepository.FindOneAsync(x => x.UserId.Equals(userId));

                if (history == null)
                    isNewCollection = true;

                if (!forceSync)
                {
                    if (history != null)
                        return history; //return existing history
                }

                var zondaHistoryApi = await this.zondaConnector.GetOperationsAsync(types: types, balanceCurrencies: balanceCurrencies, balanceTypes: accountType);
                history = mapper.Map<ZondaOperationHistoryModel, ZondaOperationHistory>(zondaHistoryApi);
                history.UserId = userId;

                if (!isNewCollection)
                    await operationsHistoryRepository.ReplaceOneAsync(history);
                else
                    await operationsHistoryRepository.InsertOneAsync(history);
                return history;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }
        }

        public async Task<ZondaTransactionHistoryModel> GetTransactionsAsync()
        {
            return await cache.GetOrAddAsync(TRANSACTIONS_CACHE_KEY, async () =>
            {
                return await zondaConnector.GetTransactionsAsync();
            });
        }

        public async Task<List<ZondaCryptoBalanceModel>> GetCryptoBalancesAsync(string[]? currencies)
        {
            var balances = new List<ZondaCryptoBalanceModel>();
            var operations = await GetOperationsAsync(balanceCurrencies: currencies);
            var transactions = await GetTransactionsAsync();
            foreach (var currency in currencies)
            {
                var currentMarket = $"{currency}-USD"; //TODO: handle it somehow
                string cacheKey = MARKET_STATS_CACHE_KEY + currentMarket;
                ZondaMarketStatsModel marketStatsModel = await cache.GetOrAddAsync(cacheKey, async () => { return await zondaConnector.GetMarketStatsAsync(currentMarket); }, TimeSpan.FromHours(6));
                ZondaCryptoBalanceModel balanceModel = new ZondaCryptoBalanceModel();
                var currencyTransactions = transactions?.Items.Where(x => x.Market.ToLower().Contains(currency.ToLower()));
                var currencyOperations = operations?.Items?.Where(x => x.Balance.Currency.ToLower() == currency.ToLower()).ToList();
                if (currencyOperations != null && currencyOperations.Any())
                {
                    balanceModel.Name = currency;
                    balanceModel.Amount = currencyOperations.Sum(x => x.Value);
                }
                if (currencyTransactions != null && currencyTransactions.Any())
                {
                    var invested = currencyTransactions
                        .Where(x => ZondaSupportedMarkets.FiatMarkets.Any(y => y.ToLower().Contains(x.Market.Split("-")[1].ToLower())))
                        .Sum(x =>
                        {
                            return Convert.ToDecimal(x.Amount) * Convert.ToDecimal(x.Rate);
                        });
                    balanceModel.Invested = invested;
                }
                balanceModel.Worth = balanceModel.Amount * marketStatsModel.Stats.R24h;
                balances.Add(balanceModel);
            }

            return balances;
        }

        /// <summary>
        /// Returns list of available wallets with totalFunds > 0.000001
        /// </summary>
        /// <returns></returns>
        public async Task<List<ZondaBalancesWalletsModel>?> GetWallets()
        {
            return await cache.GetOrAddAsync(WALLETS_CACHE_KEY, async () =>
            {
                return (await zondaConnector.GetWalletsAsync()).Balances.Where(x => x.Type == "CRYPTO" && x.TotalFunds > (decimal)0.000001).ToList();
            });
        }

        public async Task<Wallet> SyncWalletAsync(bool saveToDb = true)
        {
            var userId = "1111";
            bool isNewWallet = false;
            var wallet = await walletRepository.FindOneAsync(x => x.UserId.Equals(userId));
            if (wallet == null || string.IsNullOrEmpty(wallet.UserId))
            {
                isNewWallet = true;
                wallet = new Wallet();
                wallet.UserId = userId;
                wallet.Invested = new List<InvestedAmountWallet>();
            }

            var investedAmount = await GetInvestedAmountAsync("PLN");
            if (investedAmount != null)
                wallet.Invested = new List<InvestedAmountWallet>() { investedAmount };

            if (saveToDb)
                if (!isNewWallet)
                    await walletRepository.ReplaceOneAsync(wallet);
                else
                    await walletRepository.InsertOneAsync(wallet);

            return wallet;
        }
    }
}
