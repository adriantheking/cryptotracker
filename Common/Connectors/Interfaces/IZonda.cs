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
        /// <summary>
        /// Returns list of operations related with account
        /// </summary>
        /// <param name="types">type of operation ref:https://docs.zonda.exchange/reference/historia-operacji</param>
        /// <param name="sort">desc or asc</param>
        /// <returns></returns>
        public Task<ZondaOperationHistoryModel?> GetOperationsAsync(string[]? types = null, string sort = "DESC");
        public RestClientOptions? SetRestOptions();
    }
}
