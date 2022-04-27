using Common.Connectors.EndpointsMapping;
using Common.Connectors.Interfaces;
using Common.Options;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace Common.Connectors
{
    public class Zonda : IZonda, IDisposable
    {
        private readonly RestClient restClient;
        private readonly ZondaConnectorOptions options;

        public Zonda(IOptions<ZondaConnectorOptions> options, RestClientOptions? restClientConfiguration)
        {
            this.options = options?.Value;
            restClient = new RestClient(this.SetRestOptions());
         
        }
        public string GetTransactionsAsync()
        {
            RestRequest request = new RestRequest(ZondaEndpoints.TransactionHistoryEndpoint, Method.Get);
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
                message = publicKey + DateTime.Now.TimeOfDay.TotalSeconds;
            }
            var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            HMACSHA512 hmac = new HMACSHA512(privateKeyBytes);
            var hash = hmac.ComputeHash(messageBytes);

            return Encoding.UTF8.GetString(hash);
        }
    }
}
