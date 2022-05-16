using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Connectors.Zonda
{
    public class ZondaBalancesModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("balances")]
        public List<ZondaBalancesWalletsModel> Balances { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }
    }

    //missmatching in convention of my naming of classes - how it would like to be ZondaBalancesBalanceModel ? LOL
    public class ZondaBalancesWalletsModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("availableFunds")]
        public decimal AvailableFunds { get; set; }

        [JsonProperty("totalFunds")]
        public decimal TotalFunds { get; set; }

        [JsonProperty("lockedFunds")]
        public decimal LockedFunds { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("balanceEngine")]
        public string BalanceEngine { get; set; }
    }
}
