using Common.Connectors.EndpointsMapping;
using Common.Connectors.Interfaces;
using Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Connectors.Zonda;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace Common.Connectors
{
    public class Zonda : IZonda, IDisposable
    {
        private readonly RestClient restClient;
        private readonly ZondaConnectorOptions options;
        private readonly ILogger<Zonda> logger;
        private Object obj = new object();
        public Zonda(ILogger<Zonda> logger, IOptions<ZondaConnectorOptions> options)
        {
            this.options = options?.Value;
            restClient = new RestClient(this.options.BaseUrl);
            this.logger = logger;

        }
        public async Task<ZondaTransactionHistoryModel?> GetTransactionsAsync()
        {
            string? nextPageCursor = "start";
            ZondaTransactionHistoryModel? transactionHistory = null;
            RestRequest? request = null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(nameof(nextPageCursor), nextPageCursor);
            try
            {
                do
                {
                    request = new RestRequest(ZondaEndpoints.TransactionHistoryEndpoint + $"?query={JsonConvert.SerializeObject(parameters)}");
                    lock (obj)
                    {
                        PrepareHeaders(request);
                    }
                    var response = await restClient.ExecuteAsync(request);

                    if (response.IsSuccessful && response.Content != null)
                    {
                        if (transactionHistory == null)
                        {
                            transactionHistory = JsonConvert.DeserializeObject<ZondaTransactionHistoryModel>(response.Content);
                            nextPageCursor = transactionHistory.NextPageCursor;
                            parameters[nameof(nextPageCursor)] = nextPageCursor;
                        }
                        else
                        {
                            var transactions = JsonConvert.DeserializeObject<ZondaTransactionHistoryModel>(response.Content);
                            if (transactions != null && transactions.Items != null && transactions.Items.Any())
                            {
                                transactionHistory.Items.AddRange(transactions.Items);

                                nextPageCursor = transactions.NextPageCursor;
                                parameters[nameof(nextPageCursor)] = nextPageCursor;
                            }
                            else
                            {
                                nextPageCursor = null;
                                parameters[nameof(nextPageCursor)] = null;
                            }
                        }
                    }
                } while (nextPageCursor != null);

                return transactionHistory;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="sort">TODO: SORT property is NOT implemented</param>
        /// <returns></returns>
        public async Task<ZondaOperationHistoryModel?> GetOperationsAsync(string[]? types = null, string sort = "DESC")
        {
            ZondaOperationHistoryModel? operationHistory = null;
            RestRequest request = null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            int offset = 0;
            parameters.Add(nameof(offset), 0);
            if (types != null)
            {
                parameters.Add("types", types);
            }

            do
            {
                if (!parameters.Any())
                {
                    request = new RestRequest(ZondaEndpoints.OperationHistoryEndpoint, Method.Get);
                    lock (obj) { PrepareHeaders(request); }
                }
                else
                {
                    request = new RestRequest(ZondaEndpoints.OperationHistoryEndpoint + $"?query={JsonConvert.SerializeObject(parameters)}", Method.Get);
                    lock (obj) { PrepareHeaders(request); }
                }

                try
                {
                    var response = await restClient.ExecuteAsync(request);
                    if (response.IsSuccessful && response.Content != null)
                    {
                        if (operationHistory == null)
                        {
                            operationHistory = JsonConvert.DeserializeObject<ZondaOperationHistoryModel>(response.Content);
                            if (operationHistory != null && operationHistory.HasNextPage.HasValue && operationHistory.HasNextPage.Value)
                            {
                                parameters[nameof(offset)] = (int)parameters[nameof(offset)] + 10;
                                offset += 10;
                            }
                        }
                        else
                        {
                            var operations = JsonConvert.DeserializeObject<ZondaOperationHistoryModel>(response.Content);
                            if (operations != null && operations.Items != null && operations.Items.Any())
                                operationHistory.Items.AddRange(operations.Items);

                            if (operations != null && operations.HasNextPage.HasValue && operations.HasNextPage.Value)
                            {
                                parameters[nameof(offset)] = (int)parameters[nameof(offset)] + 10;
                                offset += 10;
                            }
                            else
                            {
                                parameters[nameof(offset)] = 0;
                                offset = 0;
                            }
                        }
                    }



                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex?.Message, ex?.InnerException);
                    throw;
                }
            } while (offset != 0);

            return operationHistory;
        }

        public async Task<ZondaBalancesModel?> GetWalletsAsync()
        {
            RestRequest request = new RestRequest(ZondaEndpoints.WalletsList);
            lock (obj) { PrepareHeaders(request); }

            try
            {
                var response = await restClient.ExecuteAsync(request);
                if(response.IsSuccessful && response.Content != null)
                {
                    var wallets = JsonConvert.DeserializeObject<ZondaBalancesModel>(response.Content);
                    return wallets;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }

            return null;
        }

        public void Dispose()
        {
            restClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Method to generating HMAC for authorized endpoints
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="isPost">Determinate is a POST or GET request</param>
        /// <param name="parameters">If request is a POST type, parameters from query are required also here</param>
        /// <returns></returns>
        private string GetHMAC(string publicKey, string privateKey, bool isPost = false, string? parameters = null)
        {
            string message = string.Empty;
            if (isPost)
            {
                message = publicKey + DateTime.Now.TimeOfDay.TotalSeconds + parameters;
            }
            else
            {
                message = publicKey + DateTimeOffset.Now.ToUnixTimeSeconds();
            }
            var privateKeyBytes = Encoding.ASCII.GetBytes(privateKey);
            var messageBytes = Encoding.ASCII.GetBytes(message);
            HMACSHA512 hmac = new HMACSHA512(privateKeyBytes);
            var hash = hmac.ComputeHash(messageBytes);

            var output = new StringBuilder();
            foreach (var h in hash)
                output.Append(h.ToString("x2"));
            return output.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="restRequest"></param>
        /// <param name="parameters">json formatted parameters</param>
        private void PrepareHeaders(RestRequest restRequest, string? parameters = null)
        {
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("API-Key", options.PublicKey);
            restRequest.AddHeader("API-Hash", GetHMAC(options.PublicKey, options.PrivateKey, parameters: parameters));
            restRequest.AddHeader("operation-id", Guid.NewGuid().ToString());
            restRequest.AddHeader("Request-Timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }

    }
}
