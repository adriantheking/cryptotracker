using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCommon.Services
{
    public class DashboardService :  IDashboardService
    {
        private readonly ILogger<DashboardService> logger;
        private readonly IZonda zondaConnector;

        public DashboardService(ILogger<DashboardService> logger, IZonda zondaConnector)
        {
            this.logger = logger;
            this.zondaConnector = zondaConnector;
        }

        public async Task<TransactionHistoryModel> GetTransactionsAsync()
        {
            var zonda = this.zondaConnector.GetTransactionsAsync();

            return new TransactionHistoryModel();
        }
    }
}
