using Models.Connectors.Zonda;

namespace Common.Services.Interfaces
{
    public interface IZondaService
    {
        /// <summary>
        /// Method is returning transaction history from Zonda exchange
        /// </summary>
        /// <returns></returns>
        Task<ZondaTransactionHistoryModel> GetTransactionsAsync();
        Task<ZondaOperationHistoryModel> GetOperationsAsync();
    }
}
