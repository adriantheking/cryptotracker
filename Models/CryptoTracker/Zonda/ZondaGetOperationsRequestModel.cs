using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.CryptoTracker.Zonda
{
    public class ZondaGetOperationsRequestModel
    {
        [JsonProperty("types")]
        public string[]? Types { get; set; }
        [JsonProperty("balanceCurrencies")]
        public string[]? BalanceCurrencies { get; set; }
    }
}
