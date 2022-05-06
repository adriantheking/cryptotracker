using Common.Connectors.Interfaces;
using Common.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Connectors.Zonda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<TransactionService> logger;
        private readonly IZonda zondaConnector;

        public TransactionService(ILogger<TransactionService> logger, IZonda zondaConnector)
        {
            this.logger = logger;
            this.zondaConnector = zondaConnector;
        }

        public async Task<ZondaTransactionHistoryModel> GetZondaTransactions()
        {
            return await this.zondaConnector.GetTransactionsAsync();
        }
    }
}
