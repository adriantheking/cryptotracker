using Models.Connectors.Zonda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.Interfaces
{
    public interface ITransactionService
    {
        /// <summary>
        /// Method is returning transaction history from Zonda exchange
        /// </summary>
        /// <returns></returns>
        Task<ZondaTransactionHistoryModel> GetZondaTransactions();
    }
}
