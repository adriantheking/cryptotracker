using Models.Connectors.Zonda;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Connectors.Interfaces
{
    public interface IZonda
    {
        /// <summary>
        /// Method returns transactions related to account
        /// </summary>
        /// <returns></returns>
        public Task<ZondaTransactionHistoryModel?> GetTransactionsAsync();
        public Task<ZondaOperationHistoryModel?> GetOperationsAsync();
        public RestClientOptions? SetRestOptions();
    }
}
