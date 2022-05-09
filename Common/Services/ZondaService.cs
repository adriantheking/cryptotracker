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
        private static readonly string ADD_FUNDS_PROP = "ADD_FUNDS";
        private static readonly string SUBTRACT_FUNDS_PROP = "SUBTRACT_FUNDS";

        public ZondaService(ILogger<ZondaService> logger, IZonda zondaConnector)
        {
            this.logger = logger;
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
            return await this.zondaConnector.GetOperationsAsync(types: types);
        }

        public async Task<ZondaTransactionHistoryModel> GetTransactionsAsync()
        {
            return await this.zondaConnector.GetTransactionsAsync();
        }
    }
}
