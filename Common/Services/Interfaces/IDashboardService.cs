using Models.Connectors.Zonda;
using Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCommon.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<TransactionHistoryModel> GetTransactionsAsync();
    }
}
