using CryptoDatabase.Repositories.Binance;
using CryptoDatabase.Repositories.Interfaces;

namespace CryptoCommon.Utilities
{
    public interface IBinanceSeed
    {
        Task Init();
    }

    public class BinanceSeed : IBinanceSeed
    {
        private List<string> supportedStables = new List<string>() { "USDT", "USDC", "BUSD"};
        private List<string> supportedCoins = new List<string>() { "BTC", "ETH", "BNB", "UNI" };
        private readonly IMongoRepository<BinanceSupportedCoins> supportedCoinsRepository;
        private readonly IMongoRepository<BinanceSupportedStables> supportedStablesRepository;

        public BinanceSeed(IMongoRepository<BinanceSupportedCoins> supportedCoinsRepository,
            IMongoRepository<BinanceSupportedStables> supportedStablesRepository)
        {
            this.supportedCoinsRepository = supportedCoinsRepository;
            this.supportedStablesRepository = supportedStablesRepository;

            
        }

        public async Task Init()
        {
            await SeedSupportedCoins();
            await SeedSupportedStables();
        }

        private async Task SeedSupportedStables()
        {
            List<BinanceSupportedStables> records = new List<BinanceSupportedStables>();
            foreach(var stable in supportedStables)
            {
                records.Add(new BinanceSupportedStables() { Name = stable });
            }

            await supportedStablesRepository.InsertManyAsync(records);
        }
        private async Task SeedSupportedCoins()
        {
            List<BinanceSupportedCoins> records = new List<BinanceSupportedCoins>();
            foreach (var stable in supportedCoins)
            {
                records.Add(new BinanceSupportedCoins() { Name = stable });
            }

            await supportedCoinsRepository.InsertManyAsync(records);
        }
    }
}
