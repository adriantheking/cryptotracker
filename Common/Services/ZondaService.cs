using Common.Connectors.Interfaces;
using Common.Services.Interfaces;
using Common.Utilities;
using Common.Utilities.Zonda;
using LazyCache;
using Microsoft.Extensions.Logging;
using Models.Connectors.Zonda;

namespace Common.Services
{
    public class ZondaService : IZondaService
    {
        private readonly ILogger<ZondaService> logger;
        private readonly IAppCache cache;
        private readonly IZonda zondaConnector;
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

        public ZondaService(ILogger<ZondaService> logger, IAppCache cache, IZonda zondaConnector)
        {
            this.logger = logger;
            this.cache = cache;
            this.zondaConnector = zondaConnector;
        }

        public async Task<decimal> GetInvestedAmountAsync()
        {
            var types = new string[] { ADD_FUNDS_PROP, SUBTRACT_FUNDS_PROP, WITHDRAWAL_SUBTRACT_FUNDS_PROP };
            var accountTypes = new string[] { "FIAT" };
            var operations = await this.GetOperationsAsync(types, accountType: accountTypes);
            try
            {
                if (operations != null && operations.Items != null)
                {
                    return operations.Items.Sum(x => x.Value).Value;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }

            return 0;
        }

        public async Task<ZondaOperationHistoryModel> GetOperationsAsync(string[]? types = null, string[]? accountType = null, string[]? balanceCurrencies = null)
        {
            string cacheKey = OPERATIONS_CACHE_KEY;
            if (types != null)
                cacheKey += string.Join("_", types);

            return await cache.GetOrAddAsync(cacheKey, async () =>
            {
                return await this.zondaConnector.GetOperationsAsync(types: types, balanceCurrencies: balanceCurrencies, balanceTypes: accountType);
            });
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
            foreach(var currency in currencies)
            {
                var currentMarket = $"{currency}-USD"; //TODO: handle it somehow
                string cacheKey = MARKET_STATS_CACHE_KEY+currentMarket;
                ZondaMarketStatsModel marketStatsModel = await cache.GetOrAddAsync(cacheKey, async () => { return await zondaConnector.GetMarketStatsAsync(currentMarket); }, TimeSpan.FromHours(6));
                ZondaCryptoBalanceModel balanceModel = new ZondaCryptoBalanceModel();
                var currencyTransactions = transactions?.Items.Where(x => x.Market.ToLower().Contains(currency.ToLower()));
                var currencyOperations = operations?.Items?.Where(x => x.Balance.Currency.ToLower() == currency.ToLower()).ToList();
                if (currencyOperations != null && currencyOperations.Any())
                {
                    balanceModel.Name = currency;
                    balanceModel.Amount = currencyOperations.Sum(x => x.Value);
                }
                if(currencyTransactions != null && currencyTransactions.Any())
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

    }
}
