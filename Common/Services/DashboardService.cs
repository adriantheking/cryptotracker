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
            //
            //If there is no wallet in database try to call to services and fill data
            //
            if (wallet == null)
            {
                return await FillWalletAsync(userid);
            }
            return wallet;
        }

        public async Task<MongoWallet> FillWalletAsync(string userId)
        {
            try
            {
                var finalWallet = new MongoWallet() //final output
                {
                    UserId = userId,
                    Invested = new List<InvestedAmountWallet>()
                };
                //BINANCE DATA
                var binanceInvestedData = await binanceService.GetInvestedAmountAsync(); //get data from binance
                var transformedBinanceObject = binanceInvestedData.Select(x => new InvestedAmountWallet() { Value = x.Value, 
                    Fiat = x.Fiat,
                    Source = nameof(IBinance),
                }).ToList(); //transform to model

                //ZONDA DATA
                var zondaInvestedData = await zondaService.GetInvestedAmountAsync(); // get data from zonda
                var transformedZondaObject = new InvestedAmountWallet() { Value = zondaInvestedData,
                    Fiat = "PLN", //TODO: handle
                    Source = nameof(IZonda) };

                finalWallet.Invested.AddRange(transformedBinanceObject);
                finalWallet.Invested.Add(transformedZondaObject);
                await walletRepository.InsertOneAsync(finalWallet);

                return finalWallet;
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
