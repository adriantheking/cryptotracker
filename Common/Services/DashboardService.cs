using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using CryptoDatabase.Repositories;
using CryptoDatabase.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Services;
using MongoWallet = CryptoDatabase.Repositories.Wallet;
namespace CryptoCommon.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ILogger<DashboardService> logger;
        private readonly IZonda zondaConnector;
        private readonly IZondaService zondaService;
        private readonly IBinance binanceConnector;
        private readonly IBinanceService binanceService;
        private readonly IMongoRepository<MongoWallet> walletRepository;

        public DashboardService(ILogger<DashboardService> logger,
            IZonda zondaConnector,
            IZondaService zondaService,
            IBinance binanceConnector,
            IBinanceService binanceService,
            IMongoRepository<MongoWallet> walletRepository)
        {
            this.logger = logger;
            this.zondaConnector = zondaConnector;
            this.zondaService = zondaService;
            this.binanceConnector = binanceConnector;
            this.binanceService = binanceService;
            this.walletRepository = walletRepository;
        }

        public async Task<MongoWallet> GetWalletAsync()
        {
            var userid = "1111"; //TODO: Handle it
            var wallet = await walletRepository.FindOneAsync(f => f.UserId.Equals(userid));
            
            return wallet;
        }

        public async Task<MongoWallet> SyncWalletAsync(string userId)
        {
            try
            {
                var zondaWallet = await zondaService.SyncWalletAsync(false);
                var binanceWallet = await binanceService.SyncWalletAsync(saveToDb: false);
                var isNewWallet = false;
                var wallet = await walletRepository.FindOneAsync(x => x.UserId.Equals(userId));
                if (wallet == null || string.IsNullOrEmpty(wallet.UserId))
                {
                    isNewWallet = true;
                    wallet = new Wallet();
                    wallet.UserId = userId;
                }
                wallet.Invested = new List<InvestedAmountWallet>();
                if(zondaWallet != null && zondaWallet.Invested != null)
                    wallet.Invested.AddRange(zondaWallet.Invested);
                if(binanceWallet != null && binanceWallet.Invested != null)
                    wallet.Invested.AddRange(binanceWallet.Invested);

                if (!isNewWallet)
                    await walletRepository.ReplaceOneAsync(wallet);
                else
                    await walletRepository.InsertOneAsync(wallet);
                return wallet;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }
        }

        public async Task<TransactionHistoryModel> GetTransactionsAsync()
        {
            var zonda = this.zondaConnector.GetTransactionsAsync();

            return new TransactionHistoryModel();
        }
    }
}
