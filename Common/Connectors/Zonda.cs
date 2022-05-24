using Common.Connectors.EndpointsMapping;
using Common.Connectors.Interfaces;
using Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Connectors.Zonda;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Common.Connectors
{
    public class Zonda : IZonda, IDisposable
    {
        private readonly ZondaConnectorOptions options;
        private readonly HttpClient httpClient;
        private readonly ILogger<Zonda> logger;
        private Object obj = new object();
        public Zonda(HttpClient httpClient, ILogger<Zonda> logger, IOptions<ZondaConnectorOptions> options)
        {
            this.options = options?.Value;
            this.httpClient = httpClient;
            this.logger = logger;

        }
        public async Task<ZondaTransactionHistoryModel?> GetTransactionsAsync()
        {
            string? nextPageCursor = "start";
            ZondaTransactionHistoryModel? transactionHistory = null;
            HttpRequestMessage request = null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(nameof(nextPageCursor), nextPageCursor);
            try
            {
                do
                {
                    request = new HttpRequestMessage(HttpMethod.Get, ZondaEndpoints.TransactionHistoryEndpoint + $"?query={JsonConvert.SerializeObject(parameters)}");
                    lock (obj)
                    {
                        PrepareHeaders(request);
                    }
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    if (response.Content != null)
                    {
                        if (transactionHistory == null)
                        {
                            transactionHistory = JsonConvert.DeserializeObject<ZondaTransactionHistoryModel>(await response.Content.ReadAsStringAsync());
                            nextPageCursor = transactionHistory.NextPageCursor;
                            parameters[nameof(nextPageCursor)] = nextPageCursor;
                        }
                        else
                        {
                            var transactions = JsonConvert.DeserializeObject<ZondaTransactionHistoryModel>(await response.Content.ReadAsStringAsync());
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
        /// <param name="types">Type of operation for example SUBSTRACT</param>
        /// <param name="balanceCurrencies">array of currencies to filter example: ["BTC"]</param>
        /// <param name="balanceTypes">array of type of balance. FIAT or CRYPTO</param>
        /// <param name="sort">TODO: SORT property is NOT implemented</param>
        /// <returns></returns>
        public async Task<ZondaOperationHistoryModel?> GetOperationsAsync(string[]? types = null, string[]? balanceCurrencies = null, string[]? balanceTypes = null, string sort = "DESC")
        {
            ZondaOperationHistoryModel? operationHistory = null;
            HttpRequestMessage request = null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            Dictionary<string, string> balanceParameters = new Dictionary<string, string>();
            
            int offset = 0;
            parameters.Add(nameof(offset), 0);
            if (types != null)
            {
                parameters.Add(nameof(types), types);
            }
            if (balanceCurrencies != null)
            {
                parameters.Add(nameof(balanceCurrencies), balanceCurrencies);
            }
            if (balanceTypes != null)
            {
                parameters.Add(nameof(balanceTypes), balanceTypes);
            }
            do
            {
                if (!parameters.Any())
                {
                    request = new HttpRequestMessage(HttpMethod.Get, ZondaEndpoints.OperationHistoryEndpoint);
                    lock (obj) { PrepareHeaders(request); }
                }
                else
                {
                    request = new HttpRequestMessage(HttpMethod.Get, ZondaEndpoints.OperationHistoryEndpoint + $"?query={JsonConvert.SerializeObject(parameters)}");
                    lock (obj) { PrepareHeaders(request); }
                }

                try
                {
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    if (response.Content != null)
                    {
                        //Checking if this is first query or not if not we dont want overrinde 
                        if (operationHistory == null)
                        {
                            operationHistory = JsonConvert.DeserializeObject<ZondaOperationHistoryModel>(await response.Content.ReadAsStringAsync());
                            if (operationHistory != null && operationHistory.HasNextPage.HasValue && operationHistory.HasNextPage.Value)
                            {
                                parameters[nameof(offset)] = (int)parameters[nameof(offset)] + 10;
                                offset += 10;
                            }
                        }
                        else
                        {
                            var operations = JsonConvert.DeserializeObject<ZondaOperationHistoryModel>(await response.Content.ReadAsStringAsync());
                            if (operations != null && operations.Items != null && operations.Items.Any())
                                operationHistory.Items.AddRange(operations.Items);

                            operationHistory.Settings = operations.Settings;
                            operationHistory.HasNextPage = operations.HasNextPage;
                            operationHistory.Limit = operations.Limit;
                            operationHistory.Offset = operations.Offset;
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ZondaEndpoints.WalletsList);
            lock (obj) { PrepareHeaders(request); }

            try
            {
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                if (response.Content != null)
                {
                    var wallets = JsonConvert.DeserializeObject<ZondaBalancesModel>(await response.Content.ReadAsStringAsync());
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

        public async Task<ZondaMarketStatsModel?> GetMarketStatsAsync(string market)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ZondaEndpoints.Stats + "/" + market);
            lock (obj) { PrepareHeaders(request); }

            try
            {
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.Content != null)
                {
                    var stats = JsonConvert.DeserializeObject<ZondaMarketStatsModel>(await response.Content.ReadAsStringAsync());
                    return stats;
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
            httpClient?.Dispose();
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
        private void PrepareHeaders(HttpRequestMessage requestMessage, string? parameters = null)
        {
            Dictionary<string, string> parametersToSend = new Dictionary<string, string>();
            parametersToSend.Add("Content-Type", "application/json");
            parametersToSend.Add("API-Key", options.PublicKey);
            parametersToSend.Add("API-Hash", GetHMAC(options.PublicKey, options.PrivateKey, parameters: parameters));
            parametersToSend.Add("operation-id", Guid.NewGuid().ToString());
            parametersToSend.Add("Request-Timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            //requestMessage.Headers.Add("Content-Type", "application/json");

            var content = JsonConvert.SerializeObject(parametersToSend);
            requestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            requestMessage.Content.Headers.Remove("traceparent");

            //requestMessage.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //requestMessage.Headers.Add("API-Key", options.PublicKey);
            //requestMessage.Headers.Add("API-Hash", GetHMAC(options.PublicKey, options.PrivateKey, parameters: parameters));
            //requestMessage.Headers.Add("operation-id", Guid.NewGuid().ToString());
            //requestMessage.Headers.Add("Request-Timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            
            
        }


    }
}
