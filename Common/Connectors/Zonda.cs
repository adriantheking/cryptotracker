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

        public Zonda(ILogger<Zonda> logger, IOptions<ZondaConnectorOptions> options)
        {
            this.options = options?.Value;
            restClient = new RestClient(this.options.BaseUrl);
            this.logger = logger;

        }
        public async Task<ZondaTransactionHistoryModel?> GetTransactionsAsync()
        {
            PrepareHeaders(restClient);
            string? nextPageCursor = null;
            ZondaTransactionHistoryModel? transactionHistory = null;
            RestRequest? request = null;
            try
            {
                do
                {
                    if (nextPageCursor == null)
                    {
                        request = new RestRequest(ZondaEndpoints.TransactionHistoryEndpoint, Method.Get);
                    }
                    else
                    {
                        request = new RestRequest(ZondaEndpoints.TransactionHistoryEndpoint + $"?{nameof(nextPageCursor)}={nextPageCursor}");
                    }
                    var response = await restClient.ExecuteAsync(request);

                    if (response.IsSuccessful && response.Content != null)
                    {
                        if (transactionHistory == null)
                        {
                            transactionHistory = JsonConvert.DeserializeObject<ZondaTransactionHistoryModel>(response.Content);
                            nextPageCursor = transactionHistory.NextPageCursor;
                        }
                        else
                        {
                            var transactions = JsonConvert.DeserializeObject<ZondaTransactionHistoryModel>(response.Content);
                            transactionHistory.Items.AddRange(transactions.Items);
                            nextPageCursor = transactionHistory.NextPageCursor;
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


        public async Task<ZondaOperationHistoryModel?> GetOperationsAsync()
        {
            PrepareHeaders(restClient);
            ZondaOperationHistoryModel? operationHistory = null;
            RestRequest request = new RestRequest(ZondaEndpoints.OperationHistoryEndpoint, Method.Get);
            try
            {
                var response = await restClient.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    operationHistory = JsonConvert.DeserializeObject<ZondaOperationHistoryModel>(response.Content);
                }

                return operationHistory;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex?.Message, ex?.InnerException);
                throw;
            }
        }

        public RestClientOptions? SetRestOptions() => new RestClientOptions()
        {
            BaseHost = options.BaseUrl,
            Timeout = options.TimeOut.Value
        };


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

        private void PrepareHeaders(RestClient restClient)
        {
            if (!restClient.DefaultParameters.Any(x => x.Name == "Accept"))
            {
                restClient.AddDefaultHeader("Accept", "application/json");
            }
            if (!restClient.DefaultParameters.Any(x => x.Name == "Content-Type"))
            {
                restClient.AddDefaultHeader("Content-Type", "application/json");
            }
            if (restClient.DefaultParameters.Any(x => x.Name == "API-Key"))
            {
                restClient.DefaultParameters.RemoveParameter("API-Key");
            }
            if (restClient.DefaultParameters.Any(x => x.Name == "API-Hash"))
            {
                restClient.DefaultParameters.RemoveParameter("API-Hash");
            }
            if (restClient.DefaultParameters.Any(x => x.Name == "operation-id"))
            {
                restClient.DefaultParameters.RemoveParameter("operation-id");
            }
            if (restClient.DefaultParameters.Any(x => x.Name == "Request-Timestamp"))
            {
                restClient.DefaultParameters.RemoveParameter("Request-Timestamp");
            }
            restClient.AddDefaultHeader("API-Key", options.PublicKey);
            restClient.AddDefaultHeader("API-Hash", GetHMAC(options.PublicKey, options.PrivateKey));
            restClient.AddDefaultHeader("operation-id", Guid.NewGuid().ToString());
            restClient.AddDefaultHeader("Request-Timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

        }

    }
}
