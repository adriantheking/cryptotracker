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

        public async Task<MongoWallet> SyncWalletAsync(string userId, MongoWallet? wallet = null)
        {
            try
            {
                return await binanceService.SyncWalletAsync();
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
