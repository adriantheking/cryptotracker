using Common.Connectors.Interfaces;
using Common.Services.Interfaces;
using Common.Utilities;
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
        private static readonly string TRANSACTION_POST_INCOME = "TRANSACTION_POST_INCOME";
        private static readonly string TRANSACTION_POST_OUTCOME = "TRANSACTION_POST_OUTCOME";
        private static readonly string WITHDRAWAL_SUBTRACT_FUNDS = "WITHDRAWAL_SUBTRACT_FUNDS";
        private static readonly string TRANSACTION_COMMISSION_OUTCOME = "TRANSACTION_COMMISSION_OUTCOME";
        private static readonly string OPERATIONS_CACHE_KEY = "OPERATIONS_";
        private static readonly string TRANSACTIONS_CACHE_KEY = "TRANSACTIONS_";

        public ZondaService(ILogger<ZondaService> logger, IAppCache cache, IZonda zondaConnector)
        {
            this.logger = logger;
            this.cache = cache;
            this.zondaConnector = zondaConnector;
        }

        public async Task<decimal> GetInvestedAmountAsync()
        {
            var types = new string[] { ADD_FUNDS_PROP, SUBTRACT_FUNDS_PROP };
            var operations = await this.GetOperationsAsync(types);
            try
            {
                if (operations != null && operations.Items != null)
                {
                    var addFundsOperations = operations.Items.Where(item => item.Type != null && item.Type.ToUpper() == ADD_FUNDS_PROP.ToUpper());
                    var subFundsOperations = operations.Items.Where(item => item.Type != null && item.Type.ToUpper() == SUBTRACT_FUNDS_PROP.ToUpper());

                    return addFundsOperations.Select(x => x.Value).Sum().Value - subFundsOperations.Select(x => x.Value).Sum().Value;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }

            return 0;
        }

        public async Task<ZondaOperationHistoryModel> GetOperationsAsync(string[]? types = null)
        {
            string cacheKey = OPERATIONS_CACHE_KEY;
            if (types != null)
                cacheKey += string.Join("_", types);

            return await cache.GetOrAddAsync(cacheKey, async () =>
            {
                return await this.zondaConnector.GetOperationsAsync(types: types);
            });
        }

        public async Task<ZondaTransactionHistoryModel> GetTransactionsAsync()
        {
            return await cache.GetOrAddAsync(TRANSACTIONS_CACHE_KEY, async () =>
            {
                return await zondaConnector.GetTransactionsAsync();
            });
        }

        public async Task<List<ZondaCryptoBalanceModel>> GetCryptoBalancesAsync()
        {
            var balances = new List<ZondaCryptoBalanceModel>();
            var transactions = await GetTransactionsAsync();
            var operations = await GetOperationsAsync(new string[] { TRANSACTION_POST_INCOME, TRANSACTION_POST_OUTCOME, WITHDRAWAL_SUBTRACT_FUNDS, TRANSACTION_COMMISSION_OUTCOME });
            var incomes = operations.Items.Where(x => x.Type == TRANSACTION_POST_INCOME && (x.Balance.Currency.Equals(CryptocurrenciesAbbr.BTC) || x.Balance.Currency.Equals(CryptocurrenciesAbbr.ETH))); //TODO: HARDCODED FOR BTC AND ETH
            var trans = transactions.Items.Where(x => (x.UserAction == "Buy" || x.UserAction == "Sell"));

            balances.Add(new ZondaCryptoBalanceModel()
            {
                Name = CryptocurrenciesAbbr.BTC,
                Amount = trans.Where(x => x.UserAction == "Buy" && (x.Market.Equals("BTC-USDT") || x.Market.Equals("BTC-PLN"))).Select(x => Convert.ToDecimal(x.Amount)).Sum() - trans.Where(x => x.UserAction == "Sell" && (x.Market.Equals("BTC-USDT") || x.Market.Equals("BTC-PLN"))).Select(x => Convert.ToDecimal(x.Amount)).Sum()//incomes.Where(x => x.Balance.Currency.Equals(CryptocurrenciesAbbr.BTC)).Sum(x => x.Value)
            });
            balances.Add(new ZondaCryptoBalanceModel()
            {
                Name = CryptocurrenciesAbbr.ETH,
                Amount = trans.Where(x => x.UserAction == "Buy" && (x.Market.Equals("ETH-USDT") || x.Market.Equals("ETH-PLN"))).Select(x => Convert.ToDecimal(x.Amount)).Sum() - trans.Where(x => x.UserAction == "Sell" && (x.Market.Equals("ETH-USDT") || x.Market.Equals("ETH-PLN"))).Select(x => Convert.ToDecimal(x.Amount)).Sum()
            });

            return balances;
        }
    }
}
