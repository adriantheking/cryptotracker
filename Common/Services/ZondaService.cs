using Common.Connectors.Interfaces;
using Common.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Connectors.Zonda;

namespace Common.Services
{
    public class ZondaService : IZondaService
    {
        private readonly ILogger<ZondaService> logger;
        private readonly IZonda zondaConnector;

        public ZondaService(ILogger<ZondaService> logger, IZonda zondaConnector)
        {
            this.logger = logger;
            this.zondaConnector = zondaConnector;
        }

        public async Task<ZondaOperationHistoryModel> GetOperationsAsync()
        {
            return await this.zondaConnector.GetOperationsAsync();
        }

        public async Task<ZondaTransactionHistoryModel> GetTransactionsAsync()
        {
            return await this.zondaConnector.GetTransactionsAsync();
        }
    }
}
