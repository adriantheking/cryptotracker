using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Connectors.Binance
{
    public class BinanceAllUserCoinsModel
    {
        [JsonProperty("coin")]
        public string Coin { get; set; }

        [JsonProperty("depositAllEnable")]
        public bool DepositAllEnable { get; set; }

        [JsonProperty("free")]
        public decimal? Free { get; set; }

        [JsonProperty("freeze")]
        public decimal? Freeze { get; set; }

        [JsonProperty("ipoable")]
        public decimal? Ipoable { get; set; }

        [JsonProperty("ipoing")]
        public decimal? Ipoing { get; set; }

        [JsonProperty("isLegalMoney")]
        public bool? IsLegalMoney { get; set; }

        [JsonProperty("locked")]
        public decimal? Locked { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("networkList")]
        public List<BinanceUserCoinModel>? NetworkList { get; set; }

        [JsonProperty("storage")]
        public decimal? Storage { get; set; }

        [JsonProperty("trading")]
        public bool Trading { get; set; }

        [JsonProperty("withdrawAllEnable")]
        public bool WithdrawAllEnable { get; set; }

        [JsonProperty("withdrawing")]
        public string Withdrawing { get; set; }
    }

    public class BinanceUserCoinModel
    {
        [JsonProperty("addressRegex")]
        public string AddressRegex { get; set; }

        [JsonProperty("coin")]
        public string Coin { get; set; }

        [JsonProperty("depositDesc")]
        public string DepositDesc { get; set; }

        [JsonProperty("depositEnable")]
        public bool DepositEnable { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("memoRegex")]
        public string MemoRegex { get; set; }

        [JsonProperty("minConfirm")]
        public int MinConfirm { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("resetAddressStatus")]
        public bool ResetAddressStatus { get; set; }

        [JsonProperty("specialTips")]
        public string SpecialTips { get; set; }

        [JsonProperty("unLockConfirm")]
        public int UnLockConfirm { get; set; }

        [JsonProperty("withdrawDesc")]
        public string WithdrawDesc { get; set; }

        [JsonProperty("withdrawEnable")]
        public bool WithdrawEnable { get; set; }

        [JsonProperty("withdrawFee")]
        public decimal? WithdrawFee { get; set; }

        [JsonProperty("withdrawIntegerMultiple")]
        public decimal? WithdrawIntegerMultiple { get; set; }

        [JsonProperty("withdrawMax")]
        public decimal? WithdrawMax { get; set; }

        [JsonProperty("withdrawMin")]
        public decimal? WithdrawMin { get; set; }

        [JsonProperty("sameAddress")]
        public bool SameAddress { get; set; }

        [JsonProperty("estimatedArrivalTime")]
        public int? EstimatedArrivalTime { get; set; }

        [JsonProperty("busy")]
        public bool Busy { get; set; }
    }
}
